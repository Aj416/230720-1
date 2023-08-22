SELECT @quizId := QuizId FROM solv.brand where `Name` = 'Wellnest' LIMIT 1;

Update solv.quiz
set Description = 'You have successfully viewed the Wellnest 101 guidelines.',
FailureMessage = 'To Solv for Wellnest you''re going to need pass the assessment. Hit the button below to brush up on your brand knowledge or try the quiz again.',
SuccessMessage = 'You''re now fully authorised with Wellnest, so go ahead, pick up your first ticket and start earning right now.'
where Id = @quizId;