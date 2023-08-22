SELECT QuizId INTO @QuizId FROM brand WHERE Name = '<brandName>';

DELETE qqo
FROM quizquestionoption AS qqo
INNER JOIN quizquestion AS qq ON qq.Id = qqo.QuestionId
WHERE qq.QuizId = @QuizId;

UPDATE brand SET QuizId = NULL where QuizId = @QuizId;

delete from quizquestion where QuizId = @QuizId;

delete from quiz where Id = @QuizId;