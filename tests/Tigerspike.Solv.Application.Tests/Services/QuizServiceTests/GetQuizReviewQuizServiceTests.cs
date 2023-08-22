using System;
using System.Collections.Generic;
using AutoMoqCore;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Application.Tests.Services.QuizServiceTests
{
	public class GetQuizReviewQuizServiceTests
	{

		private readonly AutoMoqer Mocker = new AutoMoqer();
		private readonly IQuizService SystemUnderTests; 

		public GetQuizReviewQuizServiceTests()
		{
			SystemUnderTests = Mocker.Create<QuizService>();
		}

		[Fact]
		public void ShouldReturnFalseWhenNoAnswersAreGiven()
		{
			// Arrange
			var quiz = GetQuiz();
			var answers = new List<QuizAnswerModel>();

			// Act
			var result = SystemUnderTests.GetQuizReview(quiz, answers);

			// Assert
			Assert.False(result.IsQuizPassed);
		}

		[Fact]
		public void ShouldReturnTrueWhenAllAnswersAreCorrect()
		{
			// Arrange
			var quiz = GetQuiz();
			var answers = new [] {
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0001-000000000003"), Guid.Parse("00000000-0000-0000-0001-000000000002"), Guid.Parse("00000000-0000-0000-0001-000000000001") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0002-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0002-000000000001"), Guid.Parse("00000000-0000-0000-0002-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0003-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0003-000000000002") }
				}
			};

			// Act
			var result = SystemUnderTests.GetQuizReview(quiz, answers);

			// Assert
			Assert.True(result.IsQuizPassed);
		}

		[Fact]
		public void ShouldReturnTrueWhenWithinMistakesThreshold_OneMultiChoiceMissing()
		{
			// Arrange
			var quiz = GetQuiz();
			var answers = new[] {
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0001-000000000001"), Guid.Parse("00000000-0000-0000-0001-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0002-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0002-000000000001"), Guid.Parse("00000000-0000-0000-0002-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0003-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0003-000000000002") }
				}
			};

			// Act
			var result = SystemUnderTests.GetQuizReview(quiz, answers);

			// Assert
			Assert.True(result.IsQuizPassed);
		}

		[Fact]
		public void ShouldReturnTrueWhenWithinMistakesThreshold_OneMultiChoiceWrong()
		{
			// Arrange
			var quiz = GetQuiz();
			var answers = new[] {
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0001-000000000001"), Guid.Parse("00000000-0000-0000-0001-000000000002"), Guid.Parse("00000000-0000-0000-0001-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0002-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0002-000000000001"), Guid.Parse("00000000-0000-0000-0002-000000000002") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0003-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0003-000000000002") }
				}
			};

			// Act
			var result = SystemUnderTests.GetQuizReview(quiz, answers);

			// Assert
			Assert.True(result.IsQuizPassed);
		}

		[Fact]
		public void ShouldReturnTrueWhenWithinMistakesThreshold_OneSingleChoiceWrong()
		{
			// Arrange
			var quiz = GetQuiz();
			var answers = new[] {
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0001-000000000001"), Guid.Parse("00000000-0000-0000-0001-000000000002"), Guid.Parse("00000000-0000-0000-0001-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0002-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0002-000000000001"), Guid.Parse("00000000-0000-0000-0002-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0003-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0003-000000000001") }
				}
			};

			// Act
			var result = SystemUnderTests.GetQuizReview(quiz, answers);

			// Assert
			Assert.True(result.IsQuizPassed);
		}

		[Fact]
		public void ShouldReturnFalseWhenExceededMistakesThreshold_TwoMultiChoiceMissing()
		{
			// Arrange
			var quiz = GetQuiz();
			var answers = new[] {
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0001-000000000001"), Guid.Parse("00000000-0000-0000-0001-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0002-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0002-000000000001") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0003-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0003-000000000002") }
				}
			};

			// Act
			var result = SystemUnderTests.GetQuizReview(quiz, answers);

			// Assert
			Assert.False(result.IsQuizPassed);
		}

		[Fact]
		public void ShouldReturnFalseWhenExceededMistakesThreshold_MultiAndSingleChoiceWrong()
		{
			// Arrange
			var quiz = GetQuiz();
			var answers = new[] {
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0001-000000000001"), Guid.Parse("00000000-0000-0000-0001-000000000002"), Guid.Parse("00000000-0000-0000-0001-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0002-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0002-000000000001"), Guid.Parse("00000000-0000-0000-0002-000000000002"), Guid.Parse("00000000-0000-0000-0002-000000000003") }
				},
				new QuizAnswerModel {
					Id = Guid.Parse("00000000-0000-0000-0003-000000000000"),
					Answers = new [] { Guid.Parse("00000000-0000-0000-0003-000000000003") }
				}
			};

			// Act
			var result = SystemUnderTests.GetQuizReview(quiz, answers);

			// Assert
			Assert.False(result.IsQuizPassed);
		}



		private Quiz GetQuiz()
		{
			return new Quiz
			{
				AllowedMistakes = 1,
				Questions = new [] {
					new QuizQuestion {
						Id = Guid.Parse("00000000-0000-0000-0001-000000000000"),
						IsMultiChoice = true,
						Options = new [] {
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0001-000000000001"),  Correct = true },
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0001-000000000002"),  Correct = true },
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0001-000000000003"),  Correct = true },
						}
					},
					new QuizQuestion {
						Id = Guid.Parse("00000000-0000-0000-0002-000000000000"),
						IsMultiChoice = true,
						Options = new [] {
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0002-000000000001"),  Correct = true },
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0002-000000000002"),  Correct = false },
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0002-000000000003"),  Correct = true },
						}
					},
					new QuizQuestion {
						Id = Guid.Parse("00000000-0000-0000-0003-000000000000"),
						IsMultiChoice = false,
						Options = new [] {
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0003-000000000001"),  Correct = false },
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0003-000000000002"),  Correct = true },
							new QuizQuestionOption { Id = Guid.Parse("00000000-0000-0000-0003-000000000003"),  Correct = false },
						}
					}
				}
			};
		}

	}
}