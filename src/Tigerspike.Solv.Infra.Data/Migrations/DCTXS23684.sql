SELECT @brandId := Id FROM `brand` WHERE `Name` = 'Just Eat' LIMIT 1;

update solv.brand set `Name` = 'Wellnest',
Thumbnail = 'https://assets.solvnow.com/wellnest/CNX1_Moderation.png',
Logo = 'https://assets.solvnow.com/wellnest/CNX1_Moderation.png',
VatRate = null,
PaymentRouteId = 2,
InvoicingEnabled = 0,
InvoicingDashboardEnabled = 0,
InductionDoneMessage = '## CONGRATULATIONS! \n\n You have successfully completed your Wellnest induction. You can refer to the guidelines below at anytime.',
InductionInstructions = 'Wellnest has put together this 101 guide to get you authorised to Solv for them.',
CreateTicketInstructions = null,
CreateTicketHeader = 'Initiate Chat Support',
CreateTicketSubheader = 'We are here to help you',
ContractUrl = 'https://assets.solvnow.com/wellnest/contract.txt',
ContractInternalUrl = 'https://assets.solvnow.com/wellnest/contract.txt',
TagsEnabled = 0,
MultiTagEnabled = 0,
SubTagEnabled = 0,
NpsEnabled = 0,
TicketsExportEnabled = 1,
TicketsImportEnabled = 0,
WaitMinutesToClose = 4320,
SuperSolversEnabled = 1,
Color = '#A6CB44',
PushBackToClientEnabled = 0,
SposEnabled =0,
SposEmail = null,
ProbingFormId = null
where Id = @brandId;

-- Billing details
insert into solv.billingdetails
(Id,`Name`,Email,VatNumber,CompanyNumber,Address,IsPlatformOwner,CreatedDate
)
values
(@brandId,'Concentrix',null,'GB219627101','NI037606',
'Concentrix Europe Limited
Mayfield
49 East Bridge Street
Belfast
BT1 3NR
United Kingdom',
1,UTC_TIMESTAMP()
);

-- Quiz
SELECT @QuizId := QuizId FROM `brand` WHERE Id = @brandId LIMIT 1;
SELECT @quizQuestionId := Id FROM solv.quizquestion where QuizId = @QuizId;

delete from solv.quizquestionoption where QuestionId = @quizQuestionId;

INSERT INTO `quizquestionoption` (`Id`, `QuestionId`, `Correct`, `Text`, `Order`, `Enabled`)
VALUES
	(UUID(), @quizQuestionId, 0, 'HP', 0, 0),
	(UUID(), @quizQuestionId, 1, 'Wellnest', 1, 0),
	(UUID(), @quizQuestionId, 0, 'DELL', 2, 0);

-- Induction
SELECT @inductionSectionId := Id FROM solv.inductionsection where BrandId = @brandId;

update solv.inductionsectionitem 
set Source = 'https://assets.solvnow.com/wellnest/solv-flow.html'
where SectionId = @inductionSectionId;

-- api key
update solv.apikey set `Key` = 'wellnest-1', ApplicationId = 'wellnest-1' where BrandId = @brandId;

-- Csat response configuration
INSERT IGNORE INTO `brandadvocateresponseconfig` (`Id`, `BrandId`, `IsActive`, `Type`, `Content`, `DelayInSeconds`, `ChatActionId`, `RelevantTo`)
VALUES
    (UUID(), @brandId, 1, 19, 'How would you rate this support?', NULL, @CsatChatActionId, 'Customer');