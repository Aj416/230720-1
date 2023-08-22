SET @probingFormId = UUID();

INSERT INTO `probingform` VALUES(@probingFormId, 'Please help us to direct you to the right Support Team');

SET @q1 = UUID();
SET @q2 = UUID();
SET @q3 = UUID();
SET @q4 = UUID();
SET @q5 = UUID();

INSERT INTO `probingquestion` (`Id`, `ProbingFormId`, `Text`, `Code`, `Description`, `Order`)
VALUES
	(@q1, @probingFormId, 'Is your device physically damaged?', 'Damaged device', null, 1),
	(@q2, @probingFormId, 'Is your query related to repair?', 'Repair', null, 2),
	(@q3, @probingFormId, 'Do you have an open case number?', 'Case number', 'Case number tooltip', 3),
	(@q4, @probingFormId, 'Have you tried HPSA diagnosis tools or F2 diagnostics?', 'HPSA/F2 diagnosis', 'HPSA tooltip', 4),
	(@q5, @probingFormId, 'Is your query related to a purchase?', 'Purchase', 'Purchase tooltip', 5);

INSERT INTO `probingquestionoption` (`Id`, `QuestionId`, `Text`, `Action`, `Order`)
VALUES
	(UUID(), @q1, 'Yes', 0, 1),
	(UUID(), @q1, 'No', NULL, 2),
	(UUID(), @q2, 'Yes', 0, 1),
	(UUID(), @q2, 'No', NULL, 2),
	(UUID(), @q3, 'Yes', 0, 1),
	(UUID(), @q3, 'No', NULL, 2),
	(UUID(), @q4, 'Yes', 0, 1),
	(UUID(), @q4, 'No', NULL, 2),
	(UUID(), @q5, 'Yes', 0, 1),
	(UUID(), @q5, 'No', NULL, 2);

UPDATE `brand` SET ProbingFormId = @probingFormId WHERE ShortCode = 'AMS';