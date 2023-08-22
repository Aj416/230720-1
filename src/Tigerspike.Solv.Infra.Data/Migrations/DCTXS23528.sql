UPDATE `brand` SET
	CreateTicketSubheader = CONCAT('We are here to help you \n\n', CreateTicketSubheader)
WHERE CreateTicketSubheader IS NOT NULL AND `ShortCode` <> 'AMS';

UPDATE `brand` SET
	CreateTicketSubheader = 'We are here to help you'
WHERE CreateTicketSubheader IS NULL;


