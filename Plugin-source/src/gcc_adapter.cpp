#include <iostream>
#include <thread>
#include <array>
#include <mutex>

#include "gcc_adapter.h"
#include "gcc_error.h"

//"fields"--------------------------------------------------------------------------------------------------------------

const uint16_t VENDOR_ID = 0x057e;                      // the vendor id of the adapter.
const uint16_t PRODUCT_ID = 0x0337;                     // the product id of the adapter.
const int PAYLOAD_SIZE = 37;                            // the size of a payload (read)

static struct libusb_context *session;                  // the lib usb session context.
static struct libusb_device_handle *device_handle;      // the device handle of the adapter.

static uint8_t endpoint_in;                             // the in/read endpoint address.
static uint8_t endpoint_out;                            // the out/write endpoint address.
static uint8_t controller_payload[37];                  // the read data gets written to this before being swapped.

static char last_debug_message[300];                    // the last debug message.
static void (*log_callback)(char *) = nullptr;          // the callback that is called for logging.

static std::atomic<bool> is_polling;                    // true when the threads are currently polling.
static bool ready_to_rumble;                            // kek

static void (*connected_callback)() = nullptr;          // callback when hotplug signals a connected adapter.
static void (*disconnected_callback)() = nullptr;       // callback when hotplug signals a disconnected adapter.

//"exported" functions--------------------------------------------------------------------------------------------------

extern "C" {

bool setup_debug_log(void (*log_cb)(char *)) {
    log_callback = log_cb;
    if (!initialize()) return false;

    libusb_set_log_cb(session,
                      [](libusb_context *ctx, enum libusb_log_level level, const char *str) { debug_log(str); },
                      LIBUSB_LOG_CB_CONTEXT);

    debug_log("logging was set up successfully.");
    return true;
}

bool is_device_connected() {
    if (!initialize()) return false;
    debug_log("checking for adapter device...");

    // discover devices
    libusb_device **list;
    ssize_t device_count = libusb_get_device_list(nullptr, &list);

    if (device_count < 0)
        return false;

    for (ssize_t i = 0; i < device_count; i++) {
        libusb_device *device = list[i];
        if (is_gamecube_controller_adapter(device)) {
            libusb_free_device_list(list, 1);
            return true;
        }
    }

    debug_log("adapter not found among devices");
    libusb_free_device_list(list, 1);
    return false;
}

bool is_hotplug_supported() {
    if (!initialize()) return false;
    return (libusb_has_capability(LIBUSB_CAP_HAS_HOTPLUG) != 0);
}

int setup_hotplug(void (*connected)(), void (*disconnected)()) {
    return 0;
}

int start(uint8_t swap[], void (*data_read)(), void (*polling_ended)(int)) {
    if (!initialize()) return INIT_FAILED;
    int ret;

    // discover devices
    libusb_device **list;
    libusb_device *adapter_device = nullptr;
    ssize_t device_count = libusb_get_device_list(session, &list);

    if (device_count < 0)
        return NO_DEVICES;

    for (ssize_t i = 0; i < device_count; i++) {
        libusb_device *device = list[i];
        if (is_gamecube_controller_adapter(device)) {
            debug_log("adapter found in device list.");
            adapter_device = device;
            break;
        }
    }

    // open the adapter, claim its interface, find endpoint addresses and send initial payload
    if (adapter_device != nullptr) {

        ret = libusb_open(adapter_device, &device_handle);
        if (ret != 0) {
            debug_log("opening adapter device failed.");
            libusb_free_device_list(list, 1);
            return OPEN_FAILED;
        }
        debug_log("device opened.");

        ret = libusb_claim_interface(device_handle, 0);
        if (ret != 0) {
            libusb_close(device_handle);
            libusb_free_device_list(list, 1);
            return CLAIM_FAILED;
        }
        debug_log("interface claimed.");

        libusb_config_descriptor *config;
        libusb_get_config_descriptor(adapter_device, 0, &config);

        for (uint8_t interfaces_index = 0; interfaces_index < config->bNumInterfaces; interfaces_index++) {
            const struct libusb_interface *interfaceContainer = &config->interface[interfaces_index];
            for (int i = 0; i < interfaceContainer->num_altsetting; i++) {
                const struct libusb_interface_descriptor *interface = &interfaceContainer->altsetting[i];
                for (uint8_t e = 0; e < interface->bNumEndpoints; e++) {
                    const struct libusb_endpoint_descriptor *endpoint = &interface->endpoint[e];
                    if (endpoint->bEndpointAddress & LIBUSB_ENDPOINT_IN)
                        endpoint_in = endpoint->bEndpointAddress;
                    else
                        endpoint_out = endpoint->bEndpointAddress;
                }
            }
        }

        int tmp = 0;
        unsigned char payload = 0x13; // fuck if I know what this is. that's how dolphin does it tho...
        int err = libusb_interrupt_transfer(device_handle, endpoint_out, &payload, sizeof(payload), &tmp, 16);
        debug_log("deployed initial payload.");

        if (err != 0 || tmp != sizeof(payload)) {
            libusb_close(device_handle);
            libusb_free_device_list(list, 1);
            return INITIAL_TRANSFER_FAILED;
        }

        is_polling = true;
        //input_thread
        std::thread(read, swap, data_read, polling_ended).detach();

    } else {
        libusb_free_device_list(list, 1);
        return NOT_FOUND;
    }

    libusb_free_device_list(list, 1);
    ready_to_rumble = true;
    return SUCCESS;
}

int rumble(uint8_t ports[]) {
    if (!ready_to_rumble) {
        return NOT_STARTED;
    }

    int size = 0;

    unsigned char payload[5] = {
            0x11,
            ports[0],
            ports[1],
            ports[2],
            ports[3],
    };

    debug_log("rumbling");
    int err = libusb_interrupt_transfer(device_handle, endpoint_out, payload, sizeof(payload), &size, 16);

    if (err == 0) {
        return RUMBLE_FAILED;
    } else {
        return SUCCESS;
    }
}

void reset() {
    is_polling = false;

    if (device_handle != nullptr) {
        libusb_release_interface(device_handle,0);
        libusb_close(device_handle);
        device_handle = nullptr;
    }
}

void stop() {
    reset();

    if (session != nullptr) {
        libusb_exit(session);
        session = nullptr;
    }

    debug_log("stopped.");
}

}

//"local" functions-----------------------------------------------------------------------------------------------------

static bool initialize() {
    int ret;
    if (session == nullptr) {
        ret = libusb_init(&session);
        if (ret != 0) {
            session = nullptr;
            return false;
        }
        debug_log("initialized");
    }
    return true;
}

static void debug_log(const char *message) {
    if (log_callback == nullptr) return;

    strcpy_s(last_debug_message, sizeof(last_debug_message), message);
    log_callback(last_debug_message);
}

static bool is_gamecube_controller_adapter(libusb_device *device) {
    libusb_device_descriptor descriptor;
    int ret = libusb_get_device_descriptor(device, &descriptor);
    return (ret == 0 && descriptor.idProduct == PRODUCT_ID && descriptor.idVendor == VENDOR_ID);
}

static void read(uint8_t swap[], void (*data_read)(), void (*polling_ended)(int)) {
    int payload_size = 0;
    while (is_polling) {
        int err = libusb_interrupt_transfer(
                device_handle,
                endpoint_in,
                swap,
                PAYLOAD_SIZE,
                &payload_size,
                16);

        if (err == 0 && payload_size == PAYLOAD_SIZE) {
            data_read();
        }
        else {
            debug_log("polling in reader thread failed because:");
            debug_log(libusb_error_name(err));
            polling_ended(err);
            is_polling = false;
        }

        std::this_thread::yield();
    }
    debug_log("Reader thread has stopped.");
}
