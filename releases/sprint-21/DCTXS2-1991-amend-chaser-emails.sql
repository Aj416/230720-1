SELECT @hpBrandId := Id FROM `Brand` WHERE `name` = 'HP' limit 1;

DELETE FROM `brandnotificationconfig` WHERE BrandId = @hpBrandId;

INSERT INTO `brandnotificationconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `DeliverAfterSeconds`, `Subject`, `Header`, `Body`)
VALUES
	(UUID(), @hpBrandId, 1, 0, 28800, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', '{{AdvocateFirstName}} has marked your support ticket as solved', 'Please note, if we do not hear from you within the next <b>3 days</b>, your support ticket will be closed'),
	(UUID(), @hpBrandId, 1, 0, 172800, '{{AdvocateFirstName}} is requesting to close the question: {{QuestionSummary}}', 'Your support ticket will be closed in the next 24 hours', 'Please note, if we do not hear from you within the next <b>1 day</b>, your support ticket will be closed');
