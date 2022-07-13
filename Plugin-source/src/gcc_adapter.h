#pragma once
#ifndef GCC_ADAPTER_H
#define GCC_ADAPTER_H

#include <libusb-1.0/libusb.h>

#include "export_header.h"

extern "C" {
    GAMECUBE_CONTROLLER_EXPORT bool setup_debug_log(void (*log_cb)(char*));

    GAMECUBE_CONTROLLER_EXPORT bool is_device_connected();

    GAMECUBE_CONTROLLER_EXPORT bool is_hotplug_supported();

    GAMECUBE_CONTROLLER_EXPORT int setup_hotplug(void (*connected)(), void (*disconnected)());

    GAMECUBE_CONTROLLER_EXPORT int start(uint8_t swap[], void (*data_read)(), void (*polling_ended)(int));

    GAMECUBE_CONTROLLER_EXPORT int rumble(uint8_t ports[]);

    GAMECUBE_CONTROLLER_EXPORT void reset();

    GAMECUBE_CONTROLLER_EXPORT void stop();
}

static bool initialize();

static bool is_gamecube_controller_adapter(libusb_device *device);

static void debug_log(const char *message);

static void read(uint8_t swap[], void (*data_read)(), void (*polling_ended)(int));

#endif //GCC_ADAPTER_H
