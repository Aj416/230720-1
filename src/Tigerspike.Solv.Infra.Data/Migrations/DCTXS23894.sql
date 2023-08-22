SET @probingFormId = UUID();

INSERT INTO `probingform` VALUES (@probingFormId, 'Help us connect you to the right support team by answering a few questions');

SET @q1 = UUID();
SET @q2 = UUID();

INSERT INTO `probingquestion` (`Id`, `ProbingFormId`, `Text`, `Code`, `Description`, `Order`)
VALUES
	(@q1, @probingFormId, 'Do you have an Open Case with a Case Number or are you looking for the status of your Repair?', 'CaseNumber', '## Do you have an open case number?

Your case number is a 10 digit number that begins with 5xxxxxxxxx', 1),
	(@q2, @probingFormId, 'Is your device physically damaged or not powering up?', 'DamagedDevice', 'Examples of physical damage could include cracked screen, damaged hinges, water/spillage damage, missing keys etc.', 2);

INSERT INTO `probingquestionoption` (`Id`, `QuestionId`, `Text`, `Action`, `Order`)
VALUES
	(UUID(), @q1, 'Yes', 0, 1),
	(UUID(), @q1, 'No', NULL, 2),
	(UUID(), @q2, 'Yes', 0, 1),
	(UUID(), @q2, 'No', NULL, 2);

UPDATE `brand` SET ProbingFormId = @probingFormId WHERE ShortCode = 'AMS';
