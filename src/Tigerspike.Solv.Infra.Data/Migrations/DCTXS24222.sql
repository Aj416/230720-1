SET @questionId = UUID();
SELECT @areaId := Id FROM `profilearea` WHERE `Title` = 'Create your profile' LIMIT 1;
SELECT @questionTypeId := Id FROM `profilequestiontype` WHERE `Name` = 'MultiChoice' LIMIT 1;

INSERT IGNORE INTO `solv`.`profilequestion`
(`Id`, `AreaId`, `QuestionTypeId`, `Order`, `Title`, `SubTitle`, `Enabled`, `Optional`, `Header`, `OptionsPerRow`)
VALUES (@questionId, @areaId, @questionTypeId, 4, 'What are you interested in?', 'Solvers tend to enjoy supporting the brands that do the things they love to do as well! Tell us your interests to help with matching. You can update these in your profile at any time.', 1, 0, 'YOUR INTERESTS AND SKILLS', 3);

INSERT IGNORE INTO `solv`.`profilequestionoption`
(`Id`, `QuestionId`, `Text`, `SubText`, `Enabled`, `Optional`, `Order`, `BusinessValue`)
VALUES
(UUID(), @questionId, 'Art and Design', '', 1, 0, 1, NULL),
(UUID(), @questionId, 'Coding', '', 1, 0, 2, NULL),
(UUID(), @questionId, 'Film & cinema', '', 1, 0, 3, NULL),
(UUID(), @questionId, 'Food & cooking', '', 1, 0, 4, NULL),
(UUID(), @questionId, 'Fitness', '', 1, 0, 5, NULL),
(UUID(), @questionId, 'Gaming', '', 1, 0, 6, NULL),
(UUID(), @questionId, 'Photography', '', 1, 0, 7, NULL),
(UUID(), @questionId, 'Reading & writing', '', 1, 0, 8, NULL),
(UUID(), @questionId, 'Shopping', '', 1, 0, 9, NULL),
(UUID(), @questionId, 'Socialising', '', 1, 0, 10, NULL),
(UUID(), @questionId, 'Sport', '', 1, 0, 11, NULL),
(UUID(), @questionId, 'Technology', '', 1, 0, 12, NULL),
(UUID(), @questionId, 'Travel', '', 1, 0, 13, NULL);

UPDATE `solv`.`profilequestion`
SET
`OptionsPerRow` = 2
WHERE `Header` = 'EDUCATION';