SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;

INSERT INTO `brandnotificationconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `DeliverAfterSeconds`, `Subject`, `Header`, `Body`)
VALUES
	(UUID(), @hpBrandId, 1, 0, 28800, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', '{{AdvocateFirstName}} has marked your support ticket as solved', 'Please note, if we do not hear from you within the next <b>7 days</b>, your support ticket will be closed'),
	(UUID(), @hpBrandId, 1, 0, 259200, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', 'We noticed that your support ticket is still open at your end', 'Please note, if we do not hear from you within the next <b>4 days</b>, your support ticket will be closed'),
	(UUID(), @hpBrandId, 1, 0, 432000, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', 'Your support ticket will be closed in the next 48 hours', 'Please note, if we do not hear from you within the next <b>2 days</b>, your support ticket will be closed');

-- all other brands
INSERT INTO `brandnotificationconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `DeliverAfterSeconds`, `Subject`, `Header`, `Body`)
SELECT UUID(), Id, 1, 0, 28800, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', '{{AdvocateFirstName}} has marked your support ticket as solved', 'Please note, if we do not hear from you within the next <b>3 days</b>, your support ticket will be closed'
FROM `brand` WHERE `Name` <> 'HP' AND `IsPractice` = 0;