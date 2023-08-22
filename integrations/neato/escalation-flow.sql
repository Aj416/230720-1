select @webform := Id from `TicketSource` where `name` = 'web form' limit 1;
select @chat := Id from `TicketSource` where `name` = 'chat' limit 1;
select @client := Id from `Brand` where `name` = 'neato' limit 1;

insert into `TicketEscalationConfig` values 
(UUID(), @client, NULL, NULL, 2, 1, 'Sorry, no one is available to chat at the moment.You can close this window and we ''ll email you as soon as support becomes available.'),
(UUID(), @client, @chat, 60, 2, 1, 'Sorry, no one is available to chat at the moment.You can close this window and we ''ll email you as soon as support becomes available.'),
(UUID(), @client, @webform, 28800, 2, 1, 'Sorry, no one is available to chat at the moment.You can close this window and we ''ll email you as soon as support becomes available.');
