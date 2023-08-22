SELECT @samsungBrandId := Id, @quizId := QuizId FROM `Brand` WHERE `ShortCode` = 'IT' LIMIT 1;

-- CONFIGURE SAMSUNG ITALY BRAND
UPDATE `solv`.`brand`
SET
`TagsEnabled` = 1,
`WaitMinutesToClose` = 1500,
`SuperSolversEnabled` = 1,
`SubTagEnabled` = 1,
`CategoryEnabled` = 0,
`ValidTransferEnabled` = 1,
`InductionInstructions` = 'Samsung is a global leader in consumer electronics, you will be helping users of their products with their software and hardware queries as well as general How Tos and Sales related queries. \n\n Support will be Monday to Sunday 9am to 7pm but we think busiest from Monday to Friday, lighter over the weekend. We expect to see anything from 2000-3000 customers per month to start and grow. \n\n You will earn around 2.66$ per chat ticket which will take between 3-10 mins of time to complete.',
`TicketPrice` = 2.66
WHERE `Id` = @samsungBrandId;

-- CONFIGURE TAGS FOR SAMSUNG ITALY
SET @hardware = UUID();
SET @software = UUID();
SET @salesInquiry = UUID();
SET @referToCorrectChannel = UUID();
SET @referToServiceCentre = UUID();
INSERT IGNORE INTO `solv`.`tag`
(`Id`, `BrandId`, `Name`, `Action`, `Enabled`, `Level`, `ParentTagId`, `DiagnosisEnabled`, `SposNotificationEnabled`, `Description`)
VALUES
(@hardware, @samsungBrandId, 'Hardware ', 0, 1, NULL, NULL, 1, 0, NULL),
(@software, @samsungBrandId, 'Software ', NULL, 1, NULL, NULL, 0, 0, NULL),
(@salesInquiry, @samsungBrandId, 'Sales inquiry', NULL, 1, NULL, NULL, 0, 0, NULL),
(@referToCorrectChannel, @samsungBrandId, 'Refer to correct channel', NULL, 1, NULL, NULL, 0, 0, NULL),
(@referToServiceCentre, @samsungBrandId, 'Refer to service centre', NULL, 1, NULL, NULL, 0, 0, NULL),
(UUID(), @samsungBrandId, 'Referred to SVC', NULL, 1, NULL, @software, 0, 0, NULL),
(UUID(), @samsungBrandId, 'Referred to website/self-help', NULL, 1, NULL, @software, 0, 0, NULL),
(UUID(), @samsungBrandId, 'Troubleshooting fixed', NULL, 1, NULL, @software, 0, 0, NULL),
(UUID(), @samsungBrandId, 'General query', NULL, 1, NULL, @software, 0, 0, NULL),
(UUID(), @samsungBrandId, 'Customer disconnected', NULL, 1, NULL, @software, 0, 0, NULL),
(UUID(), @samsungBrandId, 'Pending customer', NULL, 1, NULL, @software, 0, 0, NULL);

-- CONFIGURE ABANDON REASON FOR SAMSUNG ITALY
UPDATE `solv`.`abandonreason`
SET
`IsActive` = 0
WHERE `BrandId` = @samsungBrandId AND (`IsBlockedAdvocate` = 0 AND `IsAutoAbandoned` = 0 AND `IsForcedEscalation` = 0);

INSERT IGNORE INTO `solv`.`abandonreason`
(`Id`, `Name`, `BrandId`, `IsActive`, `IsForcedEscalation`, `IsBlockedAdvocate`, `IsAutoAbandoned`, `Action`, `Description`)
VALUES
(UUID(), 'I don''t have the skills/knowledge to resolve', @samsungBrandId, 1, 0, 0, 0, NULL, NULL),
(UUID(), 'Physical Damage', @samsungBrandId, 1, 0, 0, 0, 0, NULL),
(UUID(), 'Complaints', @samsungBrandId, 1, 0, 0, 0, 0, NULL),
(UUID(), 'Follow up on repair status', @samsungBrandId, 1, 0, 0, 0, 0, NULL),
(UUID(), 'Business customer', @samsungBrandId, 1, 0, 0, 0, 0, NULL),
(UUID(), 'Dangerous product', @samsungBrandId, 1, 0, 0, 0, 0, NULL);

-- CONFIGURE RESPONSE CONFIG FOR SAMSUNG ITALY
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`, `AbandonedCount`, `EscalationReason`, `IsAutoAbandoned`, `Priority`, `AuthorUserType`)
VALUES
(UUID(), @samsungBrandId, 1, 16, NULL, NULL, NULL, NULL, 2, NULL, NULL, 100, NULL),
(UUID(), @samsungBrandId, 1, 9, 'This support ticket has been open for 1 day. We''ll close this now but if you need us again please submit a new query and our support team will be very pleased to assist you.', 86400, NULL, NULL, NULL, NULL, NULL, 100, NULL);

-- CONFIGURE QUIZ FOR SAMSUNG ITALY
UPDATE `solv`.`quiz`
SET
`AllowedMistakes` = 4
WHERE `Id` = @quizId;