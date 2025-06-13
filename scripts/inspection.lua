require "logger"
require "camera"

cam = nil

function init()
  log("Starting camera...")
  cam = camera.getFirst()
  cam.startCapture()
  log("Camera started successfully")
end

function execute()
  img = cam.grab()
  
  logger.logInfo(img)
  
  -- On ferme la camera
  log("Stopping camera...")
  cam.stopCapture()
  log("Camera stopped")
end






function log(message)
    local heure = os.date("%H:%M:%S")  -- Format de l'heure : HH:MM:SS
    logger.logDebug("[" .. heure .. "] " .. tostring(message))
end

