SELECT @lazadaBrandId := Id FROM `Brand` WHERE `name` = 'Lazada' LIMIT 1;
SELECT @lazadaQuizId := QuizId FROM `Brand` WHERE `name` = 'Lazada' LIMIT 1;

UPDATE brand SET AgreementContent = NULL, AgreementHeading = NULL, IsAgreementRequired = 0,
InductionDoneMessage = '## CONGRATULATIONS

You have successfully viewed Lazada Peer 101 guidelines. To represent Lazada, you\'ll now need to take a peer knowledge check. Don\'t worry, you still pass if you get one question wrong.',

InductionInstructions = 'Here are 4 steps to complete, before you can begin solving for Lazada:

1. Review and understand all knowledge links below
2. Complete and pass the Quiz
3. Request logins through info@solvnow.com (we may need some additional details from you and logins take 48 hours to create)
4. Once you have received your logins, visit the "Software Download Page" link below to download tools remotely'
WHERE Id = @lazadaBrandId;

UPDATE quiz
SET SuccessMessage = 'Please request logins for the Lazada tools through info@solvnow.com. Note for security purposes we may need some additional info from you. Logins take 48 hours to create and once received, you can use the Systems Software Download link in your knowledge hub to download and access the Lazada tools.'
WHERE Id = @lazadaQuizId;