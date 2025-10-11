require "logger"
require "camera"

cam = nil

cameraWasStarted = true

function init()
end

function execute()
  e()
  e("bonjour")
end

function e()
  logger.logError("1 ere methode")
end

function e(mesasge)
  logger.logError("2 eme methode")
end






function log(message)
    local heure = os.date("%H:%M:%S")  -- Format de l'heure : HH:MM:SS
    logger.logDebug("[" .. heure .. "] " .. tostring(message))
end

