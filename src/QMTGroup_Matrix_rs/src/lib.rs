#[derive(Debug, Clone)]
#[repr(u8)]
pub enum ChannelType {
    Y8,
    Rgb8,
    Bgr8,
    Xrgb8,
    Rgbx8,
    Rgba8,
    Xbgr8,
    Bgrx8,
    Bgra8,
}

impl ChannelType {
    pub fn channel_count(&self) -> u8 {
        match self {
            ChannelType::Y8 => 1,
            ChannelType::Rgb8 | ChannelType::Bgr8 => 3,
            ChannelType::Xrgb8 | ChannelType::Rgbx8 | ChannelType::Rgba8 |
            ChannelType::Xbgr8 | ChannelType::Bgrx8 | ChannelType::Bgra8 => 4,
        }
    }
}

#[derive(Debug)]
pub struct Matrix {
    width: usize,
    height: usize,
    data: Vec<u8>,
    channel_type: ChannelType,
    channel_size: u8
}

impl Matrix {
    pub fn new(width: usize, height: usize, channel_type: ChannelType) -> Self {
        let channel_size = channel_type.channel_count();
        let size = width * height * (channel_size as usize);
        Matrix {
            width,
            height,
            channel_type,
            channel_size,
            data: vec![0u8; size],
        }
    }

    pub fn clone(&self) -> Self {
        Matrix {
            width: self.width,
            height: self.height,
            channel_type: self.channel_type.clone(),
            channel_size: self.channel_size,
            data: self.data.clone(),
        }
    }

    pub fn set_data(&mut self, new_data: Vec<u8>) -> Result<(), &'static str> {
        let expected_size = self.width * self.height * (self.channel_size as usize);
        if new_data.len() != expected_size {
            return Err("Matrices sizes does not match");
        }
        self.data = new_data;
        Ok(())
    }

    pub fn get_channel_type(&self) -> ChannelType {
        self.channel_type.clone()
    }

    pub fn get_channels(&self) -> u8 {
        self.channel_size
    }

    pub fn get_width(&self) -> usize {
        self.width
    }

    pub fn get_height(&self) -> usize {
        self.height
    }
}

//
// C ABI exports
//

#[unsafe(no_mangle)]
pub extern "C" fn matrix_new(width: usize, height: usize, channel_type: u32) -> *mut Matrix {
    let channel = match channel_type {
        0 => ChannelType::Y8,
        1 => ChannelType::Rgb8,
        2 => ChannelType::Bgr8,
        3 => ChannelType::Xrgb8,
        4 => ChannelType::Rgbx8,
        5 => ChannelType::Rgba8,
        6 => ChannelType::Xbgr8,
        7 => ChannelType::Bgrx8,
        8 => ChannelType::Bgra8,
        _ => ChannelType::Y8, // fallback
    };
    let matrix = Matrix::new(width, height, channel);
    Box::into_raw(Box::new(matrix))
}

#[unsafe(no_mangle)]
pub extern "C" fn matrix_free(ptr: *mut Matrix) {
    if ptr.is_null() { return; }
    unsafe { drop(Box::from_raw(ptr)) }
}

#[unsafe(no_mangle)]
pub extern "C" fn matrix_get_width(ptr: *const Matrix) -> usize {
    unsafe { (*ptr).width }
}

#[unsafe(no_mangle)]
pub extern "C" fn matrix_get_height(ptr: *const Matrix) -> usize {
    unsafe { (*ptr).height }
}

#[unsafe(no_mangle)]
pub extern "C" fn matrix_get_channel_type(ptr: *const Matrix) -> u8 {
    unsafe { (*ptr).channel_type.clone() as u8 }
}

#[unsafe(no_mangle)]
pub extern "C" fn matrix_get_channel_size(ptr: *const Matrix) -> u8 {
    unsafe { (*ptr).channel_size }
}

#[unsafe(no_mangle)]
pub extern "C" fn matrix_set_data(ptr: *mut Matrix, data: *const u8, len: usize) -> i32 {
    unsafe {
        let slice = std::slice::from_raw_parts(data, len);
        match (*ptr).set_data(slice.to_vec()) {
            Ok(_) => 0,
            Err(_) => -1,
        }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn matrix_clone(ptr: *mut Matrix) -> *mut Matrix {
    let mat = unsafe { (*ptr).clone() };
    Box::into_raw(Box::new(mat))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn it_works() {
        let _mat = Matrix::new(640, 480, ChannelType::Y8);

        let result = 4;
        assert_eq!(result, 4);
    }
}
