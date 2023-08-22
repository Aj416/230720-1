SELECT @brandId := Id FROM solv.brand where `Name` = 'Wellnest' LIMIT 1;

update solv.brand set `Name` = 'CNX1 Wellnest', `Code` = 'CNX1 Wellnest', `ShortCode` = 'WN', CreateTicketHeader = 'Chat with Wellnest' where Id = @brandId;
