SELECT @chat := Id FROM `TicketSource` WHERE `name` = 'chat' LIMIT 1;
SELECT @client := Id FROM `Brand` WHERE `name` = 'bbb' LIMIT 1;

INSERT INTO `TicketEscalationConfig` VALUES 
(UUID(), @client, NULL, NULL, NULL, 1, 'Your issue is being escalated and reallocated. Someone will be with you soon.'),
(UUID(), @client, @chat, NULL, NULL, 1, 'Your issue is being escalated and reallocated. Someone will be with you soon.');