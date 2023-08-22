using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models;
using Tigerspike.Solv.Application.Models.Profile;
using Tigerspike.Solv.Application.Services;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.Tests.Services.AdvocateApplicationServiceTests
{
	public class BaseClass
	{
		#region Properties

		protected readonly ProfileQuestionsModel QuestionsViewModel = new ProfileQuestionsModel();
		protected readonly ProfileAnswerModel AnswerViewModel = new ProfileAnswerModel();
		protected readonly ProfileAnswerModel InvalidAnswers = new ProfileAnswerModel();

		private readonly QuestionTypeModel _qtMultipleChoice = new QuestionTypeModel
		{
			Name = "MultipleChoice",
			IsMultiChoice = true,
			IsSlider = false,
			IsAllRequired = false,
		};

		private readonly QuestionTypeModel _qtSingleChoice = new QuestionTypeModel
		{
			Name = "SingleChoice",
			IsMultiChoice = false,
			IsSlider = false,
			IsAllRequired = false,
		};

		private readonly QuestionTypeModel _qtMultipleChoiceSlider = new QuestionTypeModel
		{
			Name = "MultipleChoiceSlider",
			IsMultiChoice = true,
			IsSlider = true,
			IsAllRequired = true
		};

		private readonly QuestionTypeModel _qtSingleChoiceSlider = new QuestionTypeModel
		{
			Name = "SingleChoiceSlider",
			IsMultiChoice = false,
			IsSlider = true,
			IsAllRequired = false,
		};

		private readonly QuestionTypeModel _qtTagInput = new QuestionTypeModel
		{
			Name = "TagInput",
			IsMultiChoice = true,
			IsSlider = false,
			IsAllRequired = false,
		};

		protected IAdvocateApplicationService AdvocateApplicationService;
		protected Mock<IMapper> Mapper;
		protected Mock<IMediatorHandler> Mediator;
		protected Mock<IAreaRepository> MockAreaRepository;
		protected Mock<IQuestionRepository> MockQuestionRepository;
		protected Mock<IAdvocateApplicationRepository> MockAdvocateApplicationRepository;
		protected Mock<IApplicationAnswerRepository> MockApplicationAnswerRepository;
		protected Mock<IProfileBrandRepository> MockProfileBrandRepository;
		protected Mock<IAdvocateApplicationService> MockAdvocateApplicationService;
		protected Mock<IUserRepository> MockUserRepository;
        protected Mock<IAdvocateRepository> MockAdvocateRepository;
        protected Mock<IAdvocateBrandRepository> MockAdvocateBrandRepository;

        #endregion Properties

        #region Setup

        protected BaseClass()
		{
			GenerateQuestionViewModel();
			GenerateMockAnswers();
			GenerateMockInvalidAnswers();
			BaseClassSetup();
		}

		#endregion Setup

		#region Helpers

		private void BaseClassSetup()
		{
			Mapper = new Mock<IMapper>();
			Mediator = new Mock<IMediatorHandler>();
			MockAreaRepository = new Mock<IAreaRepository>();
			MockQuestionRepository = new Mock<IQuestionRepository>();
			MockAdvocateApplicationRepository = new Mock<IAdvocateApplicationRepository>();
			MockApplicationAnswerRepository = new Mock<IApplicationAnswerRepository>();
			MockProfileBrandRepository = new Mock<IProfileBrandRepository>();
			MockAdvocateApplicationService = new Mock<IAdvocateApplicationService>();
			MockUserRepository = new Mock<IUserRepository>();
            MockAdvocateRepository = new Mock<IAdvocateRepository>();
            MockAdvocateBrandRepository = new Mock<IAdvocateBrandRepository>();

            AdvocateApplicationService = new AdvocateApplicationService(Mapper.Object, Mediator.Object,
				MockAreaRepository.Object,
				new Mock<IBrandRepository>().Object,
				MockQuestionRepository.Object, MockAdvocateApplicationRepository.Object
				, MockApplicationAnswerRepository.Object, MockProfileBrandRepository.Object, MockUserRepository.Object, MockAdvocateRepository.Object, MockAdvocateBrandRepository.Object);
		}

		private void GenerateQuestionViewModel()
		{
			QuestionsViewModel.Areas.Add(new AreaModel
			{
				Title = "Your Information",
				Questions = new List<QuestionModel>
				{
					new QuestionModel
					{
						QuestionId = new Guid("11111111-1111-1111-1111-111111111111"),
						Title = "What languages can you Solv in fluently?",
						SubTitle = "We Solv for the most exciting brands in the world",
						QuestionType = _qtMultipleChoice,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-1111-2222-1111-111111111111"),
								Text = "English"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-1111-2222-2222-111111111111"),
								Text = "German"
							}
						}
					},
					new QuestionModel
					{
						QuestionId = new Guid("11111111-2222-1111-1111-111111111111"),
						Title = "What are your favourite brands?",
						SubTitle = "Solv is about supporting the brands you are passionate about",
						QuestionType = _qtMultipleChoice,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-1111-111111111111"),
								Text = "Amazon", QuestionDependencies = new List<Guid>
								{
									new Guid("11111111-3333-1111-1111-111111111111")
								}
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-2222-111111111111"),
								Text = "Facebook"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-3333-111111111111"),
								Text = "Intel"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-4444-111111111111"),
								Text = "Samsung"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-5555-111111111111"),
								Text = "Apple"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-6666-111111111111"),
								Text = "Google"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-7777-111111111111"),
								Text = "Netflix"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-2222-2222-8888-111111111111"),
								Text = "Intel", QuestionDependencies = new List<Guid>
								{
									new Guid("11111111-4444-1111-1111-111111111111")
								}
							}
						}
					},
					new QuestionModel
					{
						QuestionId = new Guid("11111111-3333-1111-1111-111111111111"),
						Title = "How much do you like Amazon? (Optional if Amazon)",
						SubTitle = "Rate Amazon",
						QuestionType = _qtSingleChoiceSlider,
						Optional = true,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-3333-2222-1111-111111111111"),
								Text = "Rate Amazon"
							}
						}
					},
					new QuestionModel
					{
						QuestionId = new Guid("11111111-4444-1111-1111-111111111111"),
						Title = "Do you like Intel? (Optional if Microsoft)",
						SubTitle = "We love Intel!",
						QuestionType = _qtSingleChoice,
						Optional = true,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-4444-2222-1111-111111111111"),
								Text = "Yes"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-4444-2222-2222-111111111111"),
								Text = "No"
							}
						}
					}
				}
			});

			QuestionsViewModel.Areas.Add(new AreaModel
			{
				Title = "Your skills",
				Questions = new List<QuestionModel>
				{
					new QuestionModel
					{
						QuestionId = new Guid("11111111-5555-1111-1111-111111111111"),
						Title = "Select at least one way you would like to Solv?",
						SubTitle = "Solv helps customer in many different ways",
						QuestionType = _qtMultipleChoice,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-5555-2222-1111-111111111111"),
								Text = "Customer Service"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-5555-2222-2222-111111111111"),
								Text = "Simple technical assistance"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-5555-2222-3333-111111111111"),
								Text = "Complex technical support"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-5555-2222-4444-111111111111"),
								Text = "General enquired and advise"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-5555-2222-5555-111111111111"),
								Text = "Pre sales help and advice"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-5555-2222-6666-111111111111"),
								Text = "Research to help solve problems"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-5555-2222-7777-111111111111"),
								Text = "Helpdesk"
							}
						}
					},
					new QuestionModel
					{
						QuestionId = new Guid("11111111-6666-1111-1111-111111111111"),
						Title = "Please rate your selected skills",
						SubTitle = "We'd love to learn more",
						QuestionType = _qtMultipleChoiceSlider,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-6666-2222-1111-111111111111"),
								Text = "Customer service"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-6666-2222-2222-111111111111"),
								Text = "Simple technical assistance"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-6666-2222-3333-111111111111"),
								Text = "Pre sales help and advice"
							}
						}
					},
					new QuestionModel
					{
						QuestionId = new Guid("11111111-7777-1111-1111-111111111111"),
						Title = "Please rate your technical skills being sure to choose atleast one",
						SubTitle = "New lets delve a bit deeper",
						QuestionType = _qtMultipleChoiceSlider,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-1111-111111111111"),
								Text = "Basic technical knowledge",
								SubText = "Script-led troubleshooting"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-2222-111111111111"),
								Text = "Advanced tech knowledge"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-3333-111111111111"),
								Text = "Hardware"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-4444-111111111111"),
								Text = "Software"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-5555-111111111111"),
								Text = "Microsoft Office"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-6666-111111111111"),
								Text = "Consumer electronics"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-7777-111111111111"),
								Text = "Mobile devices"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-8888-111111111111"),
								Text = "Home networking"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-7777-2222-9999-111111111111"),
								Text = "Switches"
							}
						}
					}
				}
			});

			QuestionsViewModel.Areas.Add(new AreaModel
			{
				Title = "Your availability",
				Questions = new List<QuestionModel>
				{
					new QuestionModel
					{
						QuestionId = new Guid("11111111-8888-1111-1111-111111111111"),
						Title = "Ideally how much time would you like to spend solving?",
						SubTitle = "Solv as much or as little as you like",
						QuestionType = _qtSingleChoice,
						QuestionOptions = new List<QuestionOptionModel>
						{
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-8888-2222-1111-111111111111"),
								Text = "0-5 hours per week"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-8888-2222-2222-111111111111"),
								Text = "5-10 hours per week"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-8888-2222-3333-111111111111"),
								Text = "10-15 hours per week"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-8888-2222-4444-111111111111"),
								Text = "15-20 hours per week"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-8888-2222-5555-111111111111"),
								Text = "More than 20 hours"
							},
							new QuestionOptionModel
							{
								QuestionOptionId = new Guid("11111111-8888-2222-6666-111111111111"),
								Text = "As many hours as I can"
							}
						}
					}
				}
			});
		}

		private void GenerateMockAnswers()
		{
			AnswerViewModel.AdvocateApplicationId = Guid.NewGuid();

			AnswerViewModel.ApplicationAnswers = new List<ApplicationAnswerModel>
			{
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-1111-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
						{new AnswerModel {QuestionOptionId = new Guid("11111111-1111-2222-1111-111111111111")}}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-2222-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel {QuestionOptionId = new Guid("11111111-2222-2222-2222-111111111111")},
						new AnswerModel {QuestionOptionId = new Guid("11111111-2222-2222-8888-111111111111")}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-4444-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel {QuestionOptionId = new Guid("11111111-4444-2222-1111-111111111111")}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-5555-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel {QuestionOptionId = new Guid("11111111-5555-2222-3333-111111111111")},
						new AnswerModel {QuestionOptionId = new Guid("11111111-5555-2222-6666-111111111111")},
						new AnswerModel {QuestionOptionId = new Guid("11111111-5555-2222-7777-111111111111")}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-6666-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-6666-2222-1111-111111111111"), StaticAnswer = "4"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-6666-2222-2222-111111111111"), StaticAnswer = "3"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-6666-2222-3333-111111111111"), StaticAnswer = "5"}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-7777-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-1111-111111111111"), StaticAnswer = "0"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-2222-111111111111"), StaticAnswer = "3"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-3333-111111111111"), StaticAnswer = "5"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-4444-111111111111"), StaticAnswer = "5"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-5555-111111111111"), StaticAnswer = "0"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-6666-111111111111"), StaticAnswer = "4"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-7777-111111111111"), StaticAnswer = "2"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-8888-111111111111"), StaticAnswer = "1"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-9999-111111111111"), StaticAnswer = "5"}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-8888-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel {QuestionOptionId = new Guid("11111111-8888-2222-5555-111111111111")}
					}
				}
			};
		}

		private void GenerateMockInvalidAnswers()
		{
			InvalidAnswers.ApplicationAnswers = new List<ApplicationAnswerModel>
			{
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-1111-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
						{new AnswerModel {QuestionOptionId = new Guid("11111111-1111-2222-1111-111111111111")}}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-2222-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel {QuestionOptionId = new Guid("11111111-2222-2222-2222-111111111111")},
						new AnswerModel {QuestionOptionId = new Guid("11111111-2222-2222-8888-111111111111")}
					}
				},
				new ApplicationAnswerModel //Empty QuestionId
				{
					QuestionId = Guid.Empty,
					Answers = new List<AnswerModel>
					{
						new AnswerModel {QuestionOptionId = new Guid("11111111-4444-2222-1111-111111111111")}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("a1111111-5555-1111-1111-111111111111"), //Question won't be found
					Answers = new List<AnswerModel>
					{
						new AnswerModel {QuestionOptionId = new Guid("11111111-5555-2222-3333-111111111111")},
						new AnswerModel {QuestionOptionId = new Guid("11111111-5555-2222-6666-111111111111")},
						new AnswerModel {QuestionOptionId = new Guid("11111111-5555-2222-7777-111111111111")}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-6666-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel
						{
							QuestionOptionId = new Guid("11111111-6666-2222-1111-111111111111")
						}, //Slider question with no static answer
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-6666-2222-2222-111111111111"), StaticAnswer = "3"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-6666-2222-3333-111111111111"), StaticAnswer = "5"}
					}
				},
				new ApplicationAnswerModel
				{
					QuestionId = new Guid("11111111-7777-1111-1111-111111111111"),
					Answers = new List<AnswerModel>
					{
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-1111-111111111111"), StaticAnswer = "0"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-2222-111111111111"), StaticAnswer = "3"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-3333-111111111111"), StaticAnswer = "5"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-4444-111111111111"), StaticAnswer = "5"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-5555-111111111111"), StaticAnswer = "0"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-6666-111111111111"), StaticAnswer = "4"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-7777-111111111111"), StaticAnswer = "2"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-8888-111111111111"), StaticAnswer = "1"},
						new AnswerModel
							{QuestionOptionId = new Guid("11111111-7777-2222-9999-111111111111"), StaticAnswer = "5"}
					}
				}
			};
		}

		#endregion Helpers
	}
}