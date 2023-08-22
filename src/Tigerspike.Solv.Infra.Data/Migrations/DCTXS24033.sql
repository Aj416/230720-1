SELECT @quizId := QuizId FROM `Brand` WHERE `ShortCode` = 'IT' LIMIT 1;

UPDATE `solv`.`quiz`
SET
`Description` = 'You have successfully viewed the Samsung 101 guidelines. To represent Samsung, you''ll now need to take a peer knowledge check. There are 23 questions but don''t worry, you still pass if you get four questions wrong.'
WHERE `Id` = @quizId;