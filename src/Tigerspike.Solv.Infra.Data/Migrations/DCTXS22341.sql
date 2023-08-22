SELECT @languageQuestionId := Id FROM `profilequestion` WHERE Title LIKE 'What languages%' LIMIT 1;
UPDATE `profilequestionoption` SET BusinessValue = 1 WHERE QuestionId = @languageQuestionId;

SELECT @skillsQuestionId := Id FROM `profilequestion` WHERE Title LIKE 'Please rate your selected skills' LIMIT 1;
UPDATE `profilequestionoption` SET BusinessValue = 2 WHERE QuestionId = @skillsQuestionId;

UPDATE `profilequestionoption` SET BusinessValue = 2 WHERE Text = 'Complex technical support';