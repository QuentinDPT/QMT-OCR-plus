require "logger"
require "camera"

cam = nil

cameraWasRunning = true

function init()
  cam = camera.getFirst()
  
  if cam.getStatus() ~= "Started"
  then
  	log("Starting camera...")
	cameraWasRunning = false
  	cam.startCapture()
  end
  
  log("Camera started successfully")
end

function execute()
  log("Grab")
  for i = 1, 500 do
	img = cam.grab()
	--log("<img height='512px' style='display: flex;' src ='" .. img .. "' />")
  end
  log("end grab")
  
  if not cameraWasRunning
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

