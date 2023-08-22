SELECT @hpAmsQuizId := QuizId FROM `brand` WHERE `ShortCode` = 'AMS' LIMIT 1;

UPDATE `quiz`
SET `AllowedMistakes` = 3, `Description` = 'You have successfully viewed the HP 101 guidelines. To represent HP, you''ll now need to take a peer knowledge check. There are 15 questions but don''t worry, you still pass if you get three questions wrong.'
WHERE `Id` = @hpAmsQuizId;
