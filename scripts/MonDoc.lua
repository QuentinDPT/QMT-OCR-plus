require "logger"
require "camera"

cam = nil

function init()
  -- Démarrage de la camera
  cam = camera.getFirst()
  
  log("Starting camera...")
  cam.startCapture()
  log("Camera started successfully")
end

function log(message)
    local heure = os.date("%H:%M:%S")  -- Format de l'heure : HH:MM:SS
    logger.logDebug("[" .. heure .. "] " .. tostring(message))
end

function execute()
  sleep(10000)
  
  --img = cam.grab()
  img = "bonjour"
  
  logger.logInfo(img)
  
  -- On ferme la camera
  log("Stopping camera...")
  cam.stopCapture()
  log("Camera stopped")
end

