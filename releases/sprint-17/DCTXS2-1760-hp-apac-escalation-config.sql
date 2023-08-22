SELECT @hpApacBrandId := Id FROM `brand` WHERE `name` = 'HP' limit 1;
SELECT @saSourceId := Id FROM `ticketsource` WHERE `name` = 'HP_SA' limit 1;
SELECT @shpSourceId := Id FROM `ticketsource` WHERE `name` = 'HP_SHP' limit 1;
SELECT @vaSourceId := Id FROM `ticketsource` WHERE `name` = 'HP_VA' limit 1;

INSERT INTO `ticketescalationconfig` (Id, BrandId, TicketSourceId, OpenTimeInSeconds, RejectionCount, AbandonedCount, CustomerMessage)
SELECT * FROM (
	SELECT UUID(), @hpApacBrandId as BrandId, NULL, 180, 2 AS RejectionCount, 2 AS AbandonedCount, 'Your issue is being escalated and reallocated. Someone will be with you soon.' UNION
	SELECT UUID(), @hpApacBrandId, @saSourceId, 180, 2, 2, 'Your issue is being escalated and reallocated. Someone will be with you soon.' UNION
	SELECT UUID(), @hpApacBrandId, @shpSourceId, 180, 2, 2, 'Your issue is being escalated and reallocated. Someone will be with you soon.' UNION
	SELECT UUID(), @hpApacBrandId, @vaSourceId, 180, 2, 2, 'Your issue is being escalated and reallocated. Someone will be with you soon.'
) configs
WHERE BrandId IS NOT NULL;