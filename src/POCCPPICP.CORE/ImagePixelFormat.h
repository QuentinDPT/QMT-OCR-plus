#pragma once
#include <cstdint>

namespace imaging {

    /// <summary>
    /// How the image is stored in memory.
    /// </summary>
    enum class ImagePixelFormat : uint8_t {
        /// <summary>
        /// Gray scale on one byte.
        /// </summary>
        Y8,
        /// <summary>
        /// RGB organized that way on one byte each.
        /// </summary>
        RGB8,
        /// <summary>
        /// BGR organized that way on one byte each.
        /// </summary>
        BGR8,
        /// <summary>
        /// An empty byte followed by RGB organized that way on one byte each.
        /// </summary>
        XRGB8,
        /// <summary>
        /// RGB followed by an empty byte organized that way on one byte each.
        /// </summary>
        RGBX8,
        /// <summary>
        /// RGB followed by an absorbtion byte organized that way on one byte each.
        /// </summary>
        RGBA8,
        /// <summary>
        /// An empty byte followed by BGR organized that way on one byte each.
        /// </summary>
        XBGR8,
        /// <summary>
        /// BGR followed by an empty byte organized that way on one byte each.
        /// </summary>
        BGRX8,
        /// <summary>
        /// BGR followed by an absorbtion byte organized that way on one byte each.
        /// </summary>
        BGRA8,


        /// <summary>
        /// Planar or interleaved storage.
        /// </summary>
        PLANAR = 0x80
    };
}