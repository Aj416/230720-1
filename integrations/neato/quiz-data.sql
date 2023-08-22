select @BrandId := Id from `Brand` where `name` = 'Neato' limit 1;

SET @QuizId = UUID();
SET @Q1 = UUID();
SET @Q2 = UUID();
SET @Q3 = UUID();
SET @Q4 = UUID();
SET @Q5 = UUID();
SET @Q6 = UUID();
SET @Q7 = UUID();
SET @Q8 = UUID();
SET @Q9 = UUID();
SET @Q10 = UUID();

INSERT INTO `quiz` (`Id`, `Title`, `Description`, `AllowedMistakes`)
VALUES
  (
    @QuizId,
    'TEST YOUR KNOWLEDGE',
'To represent Neato on Solv, you''ll need to pass the assessment.
* You can use the resources to help and the quiz can be taken as many times as you need.
* Don''t worry, you''ll be able to get 1 wrong and retake it as many times as you like.',
    1
  );


UPDATE `Brand` SET QuizId = @QuizId
WHERE Id = @BrandId;


INSERT INTO `quizquestion` (`Id`, `QuizId`, `IsMultiChoice`, `Title`, `Order`, `Enabled`)
VALUES
	(@Q1, @QuizId, 0, 'Wi-Fi, Bluetooth and NFC are 3 standards used for:', 1, 1),
	(@Q2, @QuizId, 0, 'What is the name given to the process for connecting a BotVac to WiFi?', 2, 1),
	(@Q3, @QuizId, 0, 'LCD stands for:', 3, 1),
	(@Q4, @QuizId, 0, 'Name the exclusive feature of the D7:', 4, 1),
	(@Q5, @QuizId, 0, 'What is DSL?', 5, 1),
	(@Q6, @QuizId, 0, 'What does the \"Turn My Switch to On\" error message mean?', 6, 1),
	(@Q7, @QuizId, 0, 'Which BotVac below is not WiFi enabled?', 7, 1),
	(@Q8, @QuizId, 0, 'Which of the following could reference a customer''s broadband provider?', 8, 1),
	(@Q9, @QuizId, 0, 'What is the limit to the amount of Floor Plans that the BotVac can store?', 9, 1),
	(@Q10, @QuizId, 0, 'Lan Stands for?', 10, 1);

INSERT INTO `quizquestionoption` (`Id`, `QuestionId`, `Correct`, `Text`, `Order`, `Enabled`)
VALUES
	(UUID(), @Q1, 1, 'Wireless communication', 1, 1),
	(UUID(), @Q1, 0, 'Internet connection', 2, 1),
	(UUID(), @Q1, 0, 'Watching TV', 3, 1),
	(UUID(), @Q1, 0, 'Communication via cables', 4, 1),
	(UUID(), @Q2, 1, 'Easy Connect', 1, 1),
	(UUID(), @Q2, 0, 'Pairing mode', 2, 1),
	(UUID(), @Q2, 0, 'Linking mode', 3, 1),
	(UUID(), @Q2, 0, 'Device association', 4, 1),
	(UUID(), @Q3, 0, 'Light Controlled Diode', 1, 1),
	(UUID(), @Q3, 0, 'Laser Crystal Display', 2, 1),
	(UUID(), @Q3, 1, 'Liquid Crystal Display', 3, 1),
	(UUID(), @Q3, 0, 'Light Crystal Display', 4, 1),
	(UUID(), @Q4, 0, 'Floor Plans', 1, 1),
	(UUID(), @Q4, 1, 'Zone Cleaning', 2, 1),
	(UUID(), @Q4, 0, 'No-Go Lines', 3, 1),
	(UUID(), @Q4, 0, 'Eco/Turbo Mode', 4, 1),
	(UUID(), @Q5, 0, 'Broadband internet via fibre', 1, 1),
	(UUID(), @Q5, 0, 'Broadband internet via cable', 2, 1),
	(UUID(), @Q5, 0, 'Broadband internet via satellite', 3, 1),
	(UUID(), @Q5, 1, 'Broadband internet via phoneline', 4, 1),
	(UUID(), @Q6, 1, 'Depleted battery', 1, 1),
	(UUID(), @Q6, 0, 'Charge Base issue', 2, 1),
	(UUID(), @Q6, 0, 'WiFi Issue', 3, 1),
	(UUID(), @Q6, 0, 'The dirt bin is full', 4, 1),
	(UUID(), @Q7, 0, 'D3', 1, 1),
	(UUID(), @Q7, 1, 'D80', 2, 1),
	(UUID(), @Q7, 0, 'D7', 3, 1),
	(UUID(), @Q7, 0, 'D4', 4, 1),
	(UUID(), @Q8, 0, 'LAN', 1, 1),
	(UUID(), @Q8, 0, 'WAN', 2, 1),
	(UUID(), @Q8, 0, 'VLAN', 3, 1),
	(UUID(), @Q8, 1, 'ISP', 4, 1),
	(UUID(), @Q9, 0, '5', 1, 1),
	(UUID(), @Q9, 0, '8', 2, 1),
	(UUID(), @Q9, 1, '3', 3, 1),
	(UUID(), @Q9, 0, '4', 4, 1),
	(UUID(), @Q10, 0, 'Local Asynchronous Network', 1, 1),
	(UUID(), @Q10, 0, 'Light Are Network', 2, 1),
	(UUID(), @Q10, 1, 'Local Area Network', 3, 1);
