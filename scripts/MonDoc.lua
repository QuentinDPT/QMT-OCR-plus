require "logger"
require "camera"

cam = nil

cameraWasStarted = true

function init()
  cam = camera.getFirst()
  
  if cam.getStatus() ~= "Started"
  then
  	log("Starting camera...")
	cameraWasStarted = false
  	cam.startCapture()
  end
  
  log("Camera started successfully")
end

function execute()
  for i = 1, 10 do
	img = cam.grab()
	log("<img height='512px' style='display: flex;' src ='" .. img .. "' />")
  end
  
  if not cameraWasStarted
  then
	log("Stopping camera...")
	cam.stopCapture()
	log("Camera stopped")
  end
end






function log(message)
    local heure = os.date("%H:%M:%S")  -- Format de l'heure : HH:MM:SS
    logger.logDebug("[" .. heure .. "] " .. tostring(message))
end

