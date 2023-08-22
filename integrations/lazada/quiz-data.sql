select @BrandId := Id from `Brand` where `name` = 'Lazada' limit 1;


SET
  @QuizId = UUID();
SET
  @Q1 = UUID();
SET
  @Q2 = UUID();
SET
  @Q3 = UUID();
SET
  @Q4 = UUID();
SET
  @Q5 = UUID();
SET
  @Q6 = UUID();

# Add quiz
INSERT INTO
  `Quiz` (
    `Id`,
    `Title`,
    `Description`,
    `AllowedMistakes`
  )
VALUES
  (
    @QuizId,
    'TEST YOUR KNOWLEDGE',
'To represent Lazada on Solv, you''ll need to pass the assessment.
* You can use the resources to help and the quiz can be taken as many times as you need.
* Don''t worry, you''ll be able to get 1 wrong and retake it as many times as you like.',
    1
  );


UPDATE `Brand` SET QuizId = @QuizId
WHERE Id = @BrandId;

# Add questions
INSERT INTO
  `QuizQuestion` (
    `Id`,
    `QuizId`,
    `IsMultiChoice`,
    `Title`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    @Q1,
    @QuizId,
    1,
    'Cancellation button will not be available if:',
    1,
    1
  );
INSERT INTO
  `QuizQuestion` (
    `Id`,
    `QuizId`,
    `IsMultiChoice`,
    `Title`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    @Q2,
    @QuizId,
    1,
    'System initiated cancellations may occur due to:',
    2,
    1
  );
INSERT INTO
  `QuizQuestion` (
    `Id`,
    `QuizId`,
    `IsMultiChoice`,
    `Title`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    @Q3,
    @QuizId,
    1,
    'Failed deliveries can occur due to:',
    3,
    1
  );
INSERT INTO
  `QuizQuestion` (
    `Id`,
    `QuizId`,
    `IsMultiChoice`,
    `Title`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    @Q4,
    @QuizId,
    1,
    'Customers can avoid failed deliveries by:',
    4,
    1
  );
INSERT INTO
  `QuizQuestion` (
    `Id`,
    `QuizId`,
    `IsMultiChoice`,
    `Title`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    @Q5,
    @QuizId,
    1,
    'When can an item be returned:',
    5,
    1
  );
INSERT INTO
  `QuizQuestion` (
    `Id`,
    `QuizId`,
    `IsMultiChoice`,
    `Title`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    @Q6,
    @QuizId,
    0,
    'How many days will it take for customer to receive a refund on their debit card:',
    6,
    1
  );
# Add options
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q1,
    'Item is already packed by seller',
    1,
    1,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q1,
    'Item is handed over to the courier',
    1,
    2,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q1,
    'Item is scheduled for dispatch and out for delivery',
    1,
    3,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q1,
    'Item to be cancelled has a shipping feed',
    0,
    4,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q2, 'System error', 1, 1, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q2, 'Failed deliveries', 1, 2, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q2,
    'Item to be delivered is incorrect',
    0,
    3,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q2, 'Out of stock', 1, 4, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q3, 'Refused at doorstep', 1, 1, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q3, 'Customer cancelled', 0, 2, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q3, 'Wrong or bad address', 1, 3, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q3, 'Customer unreachable', 1, 4, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q4,
    'For delivery address that is located in a private establishment, ensure that customer will coordinate with the receptionist or securiry guard on duty if they are expecting a package',
    1,
    1,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q4,
    'Before placing an order, customer to ensure that they have updated address registered on their Lazada account',
    1,
    2,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q4,
    'Keep their lines open if they are expecting a package from Lazada as the courier are likely to contact or send you a message if they have a scheduled delivery',
    1,
    3,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q4,
    'For cash on delivery transactions, the payment should be ready upon delivery',
    1,
    4,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q5,
    'If the item the customer received is not working properly',
    1,
    1,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q5,
    'If the item is not as advertised',
    1,
    2,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q5,
    'If the wrong item was delivered',
    1,
    3,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (
    UUID(),
    @Q5,
    'If the customer ordered thr wrong device',
    0,
    4,
    1
  );
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q6, '24 hours', 0, 1, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q6, '5 to 45 banking days', 1, 2, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q6, '5 to 12 banking days', 0, 3, 1);
INSERT INTO
  `QuizQuestionOption` (
    `Id`,
    `QuestionId`,
    `Text`,
    `Correct`,
    `Order`,
    `Enabled`
  )
VALUES
  (UUID(), @Q6, '1 to 2 business days', 0, 4, 1);