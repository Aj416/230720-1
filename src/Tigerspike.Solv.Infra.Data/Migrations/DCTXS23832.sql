SET @probingFormId = UUID();

INSERT INTO `probingform` VALUES (@probingFormId, 'Help us connect you to the right support team by answering the following question');

SET @q1 = UUID();

INSERT INTO `probingquestion` (`Id`, `ProbingFormId`, `Text`, `Code`, `Description`, `Order`)
VALUES
	(@q1, @probingFormId, 'Do you have an Open Case with a Case Number or are you looking for the status of your Repair?', 'CaseNumber', '## Do you have an open case number?\n\nYour case number is a 10 digit number that begins with 5xxxxxxxxx', 1);

INSERT INTO `probingquestionoption` (`Id`, `QuestionId`, `Text`, `Action`, `Order`, `Value`)
VALUES
	(UUID(), @q1, 'Yes', 1, 1, 'https://wa.me/912261014560'),
	(UUID(), @q1, 'No', NULL, 2, NULL);

UPDATE `brand` SET ProbingFormId = @probingFormId WHERE ShortCode = 'IND';
