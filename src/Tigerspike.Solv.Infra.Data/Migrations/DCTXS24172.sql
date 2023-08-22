SET @areaId = UUID();
SET @questionId = UUID();
SELECT @questionTypeId := Id FROM `profilequestiontype` WHERE `Name` = 'SingleChoice' LIMIT 1;

INSERT IGNORE INTO `solv`.`profilearea`
(`Id`, `Order`, `Title`, `Enabled`)
VALUES (@areaId, 1, 'Create your profile', 1);

INSERT IGNORE INTO `solv`.`profilequestion`
(`Id`, `AreaId`, `QuestionTypeId`, `Order`, `Title`, `SubTitle`, `Enabled`, `Optional`, `Header`)
VALUES (@questionId, @areaId, @questionTypeId, 2, 'What''s the highest level of education you have completed?', '', 1, 0, 'EDUCATION');

INSERT IGNORE INTO `solv`.`profilequestionoption`
(`Id`, `QuestionId`, `Text`, `SubText`, `Enabled`, `Optional`, `Order`, `BusinessValue`)
VALUES
(UUID(), @questionId, 'Don''t know', '', 1, 0, 1, NULL),
(UUID(), @questionId, 'No formal schooling', '', 1, 0, 2, NULL),
(UUID(), @questionId, 'Some high school', '', 1, 0, 3, NULL),
(UUID(), @questionId, 'High school graduate', '', 1, 0, 4, NULL),
(UUID(), @questionId, 'Some college, no degree', '', 1, 0, 5, NULL),
(UUID(), @questionId, 'Bachelor degree', '', 1, 0, 6, NULL),
(UUID(), @questionId, 'Master''s degree', '', 1, 0, 7, NULL),
(UUID(), @questionId, 'Doctorate', '', 1, 0, 8, NULL);