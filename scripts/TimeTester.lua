require "time"
require "logger"

-- Initialisation du script
function init()
	-- Votre code d'initialisation ici
	-- Ce code est executé une seule fois.
  
    logger.logInfo("time from")
	t1 = time.from(1970,01,01)
  
	logger.logDebug(t1.day)
  
    logger.logInfo("time now")
	t2 = time.now()
end

-- Boucle d'execution automatique
function execute()
	-- Votre code d'execution ici
end