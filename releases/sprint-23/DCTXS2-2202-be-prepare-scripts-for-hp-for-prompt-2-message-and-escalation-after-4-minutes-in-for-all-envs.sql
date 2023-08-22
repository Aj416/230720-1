SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;

UPDATE `brand` SET PushBackToClientEnabled = 1 WHERE Id = @hpBrandId;

SELECT @chatActionId := UUID();
INSERT INTO `chataction` (`Id`, `Type`, `IsBlocking`)
VALUES (@chatActionId, 4, 1);

SELECT @chatActionOptionId := UUID();
INSERT INTO `chatactionoption` (`Id`, `ChatActionId`, `Label`, `Value`, `IsSuggested`)
VALUES
	(@chatActionOptionId, @chatActionId, 'Chat with us on WhatsApp', '3', 1);

SELECT @sideEffectId := UUID();
INSERT INTO `chatsideeffect` (`Id`, `ChatActionOptionId`, `Effect`, `Value`)
VALUES
	(UUID(), @chatActionOptionId, 1, 'https://wa.me/912261014560');


INSERT INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `RelevantTo`, `ChatActionId`)
VALUES
	(UUID(), @hpBrandId, 1, 3, 'The wait time is a little longer than expected. You are prioritised for response but, should you wish to try another support channel, please click below', 240, 'Customer', @chatActionId);
