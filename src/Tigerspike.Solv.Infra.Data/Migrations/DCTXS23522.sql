UPDATE `ticketmetadataitem`, `brandformfield`, `ticket`
SET
  `ticketmetadataitem`.`Order` = `brandformfield`.`Order`,
  `ticketmetadataitem`.`BrandFormFieldId` = `brandformfield`.`Id`
WHERE
  `ticketmetadataitem`.`TicketId` = `ticket`.`Id` AND
  `ticket`.`BrandId` = `brandformfield`.`BrandId` AND
  `ticketmetadataitem`.`Key` = `brandformfield`.`Name`;