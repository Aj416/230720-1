SELECT @neatoId := Id FROM `brand` WHERE `Name` = 'Neato' LIMIT 1;

INSERT IGNORE INTO `ticketsource` (`Name`)
VALUES ('Web Service');

SELECT @neatoWebServiceSourceId := Id FROM `ticketsource` WHERE `Name` = 'Web Service' LIMIT 1;

INSERT IGNORE INTO `ticketescalationconfig` (`Id`, `BrandId`, `TicketSourceId`, `OpenTimeInSeconds`, `RejectionCount`, `AbandonedCount`, `CustomerMessage`)
VALUES (UUID(), @neatoId, @neatoWebServiceSourceId, NULL, 3, 2, 'Your issue is being escalated and reallocated. Someone will be with you soon.');

UPDATE `ticketescalationconfig`
SET `AbandonedCount` = 2
WHERE `BrandId` = @neatoId;