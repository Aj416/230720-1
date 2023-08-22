SET @questionId = UUID();
SET @questionOptionComboId1 = UUID();
SET @questionOptionComboId2 = UUID();
SET @questionOptionComboId3 = UUID();
SET @questionTypeId = UUID();
SELECT @areaId := Id FROM `profilearea` WHERE `Title` = 'Create your profile' LIMIT 1;

INSERT IGNORE INTO `solv`.`profilequestiontype`
(`Id`, `Name`, `IsMultiChoice`, `IsSlider`, `IsAllRequired`)
VALUES
(@questionTypeId, 'MultiOptionSet', 1, 0, 1);

INSERT IGNORE INTO `solv`.`profilequestion`
(`Id`, `AreaId`, `QuestionTypeId`, `Order`, `Title`, `SubTitle`, `Enabled`, `Optional`, `Header`, `OptionsPerRow`)
VALUES (@questionId, @areaId, @questionTypeId, 1, 'When can you Solv?', 'Let us know your availability to help us find the best brands for you. You can update this at anytime in your profile.', 1, 0, 'SOLV TIME', 4);

INSERT IGNORE INTO `solv`.`profilequestionoptioncombo`
(`Id`, `ComboOptionTitle`, `ComboOptionType`, `QuestionId`, `Order`)
VALUES
(@questionOptionComboId1, 'DAY(S) OF THE WEEK', 1, @questionId, 1),
(@questionOptionComboId2, 'HOURS PER WEEK', 0, @questionId, 2),
(@questionOptionComboId3, 'TIME OF THE DAY', 1, @questionId, 3);

INSERT IGNORE INTO `solv`.`profilequestionoption`
(`Id`, `QuestionId`, `Text`, `SubText`, `Enabled`, `Optional`, `Order`, `BusinessValue`, `QuestionOptionComboId`)
VALUES
(UUID(), @questionId, 'Monday', '', 1, 0, 1, NULL, @questionOptionComboId1),
(UUID(), @questionId, 'Tuesday', '', 1, 0, 2, NULL, @questionOptionComboId1),
(UUID(), @questionId, 'Wednesday', '', 1, 0, 3, NULL, @questionOptionComboId1),
(UUID(), @questionId, 'Thursday', '', 1, 0, 4, NULL, @questionOptionComboId1),
(UUID(), @questionId, 'Friday', '', 1, 0, 5, NULL, @questionOptionComboId1),
(UUID(), @questionId, 'Saturday', '', 1, 0, 6, NULL, @questionOptionComboId1),
(UUID(), @questionId, 'Sunday', '', 1, 0, 7, NULL, @questionOptionComboId1),
(UUID(), @questionId, '1 - 10 hours', '', 1, 0, 1, NULL, @questionOptionComboId2),
(UUID(), @questionId, '11 - 20 hours', '', 1, 0, 2, NULL, @questionOptionComboId2),
(UUID(), @questionId, '21 - 30 hours', '', 1, 0, 3, NULL, @questionOptionComboId2),
(UUID(), @questionId, '30+ hours', '', 1, 0, 4, NULL, @questionOptionComboId2),
(UUID(), @questionId, 'Morning', '', 1, 0, 1, NULL, @questionOptionComboId3),
(UUID(), @questionId, 'Afternoon', '', 1, 0, 2, NULL, @questionOptionComboId3),
(UUID(), @questionId, 'Evening', '', 1, 0, 3, NULL, @questionOptionComboId3),
(UUID(), @questionId, 'After midnight', '', 1, 0, 4, NULL, @questionOptionComboId3);
