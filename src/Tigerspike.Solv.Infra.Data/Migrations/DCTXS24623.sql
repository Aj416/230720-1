select @quizid := QuizId from solv.brand where name = 'Dell';

Update solv.quiz set Description = 'You have successfully viewed the Dell 101 guidelines. To represent Dell, you''ll now need to take a peer knowledge check. There are 15 questions and you need to answer all of them correctly in order to pass.'
where ID = @quizid;