SET @probingFormId = UUID();

INSERT INTO `probingform` VALUES (@probingFormId, 'Please help us to direct you to the right Support Team');

SET @q1 = UUID();
SET @q2 = UUID();
SET @q3 = UUID();

INSERT INTO `probingquestion` (`Id`, `ProbingFormId`, `Text`, `Code`, `Description`, `Order`)
VALUES
	(@q1, @probingFormId, 'Do you have an Open Case with a Case Number or are you looking for the status of your Repair?', 'CaseNumber', 'Do you have an open case number?\n    Your case number is a 10 digit number that begins with 5xxxxxxxxx', 1),
	(@q2, @probingFormId, 'Are you experiencing a hardware related issue such as:Loud Noise, Screen Flickering, Unit not Powering on, Overheating?', 'DamagedDevice', NULL, 2),
	(@q3, @probingFormId, 'Is your PC Booting to the Windows start screen? Can you log in?', 'WindowsLogin', NULL, 3);

INSERT INTO `probingquestionoption` (`Id`, `QuestionId`, `Text`, `Action`, `Order`)
VALUES
	(UUID(), @q1, 'Yes', 0, 1),
	(UUID(), @q1, 'No', NULL, 2),
	(UUID(), @q2, 'Yes', 0, 1),
	(UUID(), @q2, 'No', NULL, 2),
	(UUID(), @q3, 'Yes', NULL, 1),
	(UUID(), @q3, 'No', 0, 2);

UPDATE `brand` SET ProbingFormId = @probingFormId WHERE ShortCode = 'AMS';