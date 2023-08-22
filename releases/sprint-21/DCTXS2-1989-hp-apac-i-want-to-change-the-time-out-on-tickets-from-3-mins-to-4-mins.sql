UPDATE ticketescalationconfig t
	INNER JOIN brand b ON t.BrandId = b.Id
SET t.`OpenTimeInSeconds` = 240
WHERE b.Name = 'HP';