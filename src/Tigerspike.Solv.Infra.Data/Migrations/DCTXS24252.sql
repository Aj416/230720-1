SELECT @neatoBrandId := Id FROM `Brand` WHERE `name` = 'Neato' limit 1;
INSERT INTO `abandonreason` (Id, `Name`, BrandID, IsActive, IsForcedEscalation, IsBlockedAdvocate, IsAutoAbandoned,`Action`,Description)
values(
	UUID(), 'Account/Data deletion', @neatoBrandId, 1, 0 , 0, 0 , 0, null
)