UPDATE `ticketstatushistory`, `ticket`
SET
  `ticketstatushistory`.`Level` = `ticket`.`Level`
WHERE
  `ticketstatushistory`.`CreatedDate` >= `ticket`.`EscalatedDate` AND
  `ticketstatushistory`.`TicketId` = `ticket`.`Id`;


UPDATE `ticketstatushistory`
SET
  `ticketstatushistory`.`Level` = 1
WHERE
  `ticketstatushistory`.`Level` = 0;