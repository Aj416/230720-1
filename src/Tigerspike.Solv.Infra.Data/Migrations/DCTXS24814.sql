SELECT @brand_Id := Id , @quizId := QuizId FROM solv.brand where Name = 'HP AMS Print';

update solv.brand set 
`Name` = 'HP IND Print',
`Code` = 'HP India Print',
ShortCode = 'HP IND PRT',
InductionDoneMessage = '## CONGRATULATIONS! \n\n You have successfully viewed the HP IND Print 101 guidelines. To represent HP IND Print, you''ll now need to take a peer knowledge check.',
InductionInstructions = 'HP IND Print has put together this 101 guide to get you authorised to Solv for them. \n\n **NOTE: You must not copy, share or duplicate any of the contents of this section!** \n\n HP IND Print is a global leader in home computing, you will be helping users of their products with out of warranty support, so it will be technical in nature on hardware, software and networking type questions. \n\n Support will be 24/7 but we think busiest from Monday to Friday, lighter over the weekend. We expect to see anything from 2000-3000 customers per month to start and grow. \n\n You will earn around $2.80 per chat ticket which will take between 3-10 mins of time to complete.'
where Id = @brand_Id;

update solv.quiz set FailureMessage = 'To Solv for HP India you’re going to need pass the quiz. The good news is you can try it multiple times. Brush up on your brand knowledge or try the quiz again'
where Id = @quizId;

update solv.apikey set `Key` = 'hp-ind-print-1' , ApplicationId = 'hp-ind-print-1'
where BrandId = @Brand_Id;