// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormLayoutServiceTests.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   The form layout service tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data.Providers.Tests.FieldsModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Rhino.Mocks;

    using Startitecture.Core;
    using Startitecture.Orm.Common;
    using Startitecture.Orm.Mapper;
    using Startitecture.Orm.Sql;
    using Startitecture.Orm.Repository;
    using Startitecture.Orm.Testing.RhinoMocks;

    /// <summary>
    /// The form layout service tests.
    /// </summary>
    [TestClass]
    public class FormLayoutServiceTests
    {
        /// <summary>
        /// The number generator.
        /// </summary>
        private static readonly Random NumberGenerator = new Random();

        /// <summary>
        /// The get layouts test.
        /// </summary>
        [TestMethod]
        public void GetLayouts_FormLayoutsByFormVersionWithId_MatchesExpected()
        {
            var form = new Form("My Form", 43987);
            var createdBy = new User("theuser") { FirstName = "My", LastName = "Name" };
            var formVersion = new FormVersion(form, 4, createdBy, DateTimeOffset.Now, 54987)
            {
                Footer = "Mah footer",
                Instructions = "Fill this out",
                Header = "Mah Header",
                IsActive = true,
                Title = "Best Form Evar"
            };

            var layoutRepository = MockRepository.GenerateMock<IFormLayoutRepository>();
            var versionId = formVersion.FormVersionId.GetValueOrDefault();
            var expected = new List<FormLayout>
                               {
                                   new FormLayout(versionId, 8437)
                                       {
                                           Footer = "Mah Footer for this layout",
                                           Instructions = "Fill in my stuffs",
                                           IsActive = true,
                                           Name = "Layout1"
                                       },
                                   new FormLayout(versionId, 4356)
                                       {
                                           Footer = "Mah Footer for this other layout",
                                           Instructions = "Fill in my stuffs...",
                                           IsActive = true,
                                           Name = "Layout2"
                                       },
                                   new FormLayout(versionId, 89764)
                                       {
                                           Footer = "Mah Footer for this new layout",
                                           Instructions = "Fill in my stuffs?",
                                           IsActive = false,
                                           Name = "Layout3"
                                       },
                                   new FormLayout(versionId, 982034)
                                       {
                                           Footer = "Mah Footer for this last layout",
                                           Instructions = "Fill in my stuffs!",
                                           IsActive = true,
                                           Name = "Layout4"
                                       }
                               };

            layoutRepository.Stub(repository => repository.GetLayouts(Arg<FormVersion>.Matches(version => version == formVersion))).Return(expected);

            var target = new FormLayoutService(layoutRepository);
            var actual = target.GetLayouts(formVersion).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get layouts test.
        /// </summary>
        [TestMethod]
        public void GetLayouts_FormLayoutsByFormVersionWithoutId_ThrowsException()
        {
            var form = new Form("My Form", 43987);
            var createdBy = new User("theuser") { FirstName = "My", LastName = "Name" };
            var formVersion = new FormVersion(form)
            {
                Footer = "Mah footer",
                Instructions = "Fill this out",
                Header = "Mah Header",
                IsActive = true,
                Title = "Best Form Evar"
            };

            formVersion.UpdateChangeTracking(createdBy);

            var layoutRepository = MockRepository.GenerateMock<IFormLayoutRepository>();
            var versionId = formVersion.FormVersionId.GetValueOrDefault();
            var expected = new List<FormLayout>
                               {
                                   new FormLayout(versionId, 8437)
                                       {
                                           Footer = "Mah Footer for this layout",
                                           Instructions = "Fill in my stuffs",
                                           IsActive = true,
                                           Name = "Layout1"
                                       },
                                   new FormLayout(versionId, 4356)
                                       {
                                           Footer = "Mah Footer for this other layout",
                                           Instructions = "Fill in my stuffs...",
                                           IsActive = true,
                                           Name = "Layout2"
                                       },
                                   new FormLayout(versionId, 89764)
                                       {
                                           Footer = "Mah Footer for this new layout",
                                           Instructions = "Fill in my stuffs?",
                                           IsActive = false,
                                           Name = "Layout3"
                                       },
                                   new FormLayout(versionId, 982034)
                                       {
                                           Footer = "Mah Footer for this last layout",
                                           Instructions = "Fill in my stuffs!",
                                           IsActive = true,
                                           Name = "Layout4"
                                       }
                               };

            layoutRepository.Stub(repository => repository.GetLayouts(Arg<FormVersion>.Matches(version => version == formVersion))).Return(expected);

            var target = new FormLayoutService(layoutRepository);

            try
            {
                var actual = target.GetLayouts(formVersion).ToList();
                Assert.Fail("An exception should have been thrown.");
            }
            catch (BusinessException ex)
            {
                Assert.AreEqual(formVersion, ex.TargetEntity);
                Assert.IsTrue(ex.Message.Contains(FieldsMessages.IdValueLessThanOne));
            }
        }

        /// <summary>
        /// The add layout test.
        /// </summary>
        [TestMethod]
        public void AddLayout_NewFormLayoutToFormVersion_MatchesExpected()
        {
            var layoutRepository = MockRepository.GenerateMock<IFormLayoutRepository>();
            var target = new FormLayoutService(layoutRepository);
            var form = new Form("My form", 439857);
            var createdBy = new User("account") { FirstName = "First", LastName = "Last" };
            var formVersion = new FormVersion(form, 1, createdBy, DateTimeOffset.Now, 434528) { Title = "The title" };
            var versionId = formVersion.FormVersionId.GetValueOrDefault();
            var expected = new FormLayout(versionId) { Name = "New Layout" };
            var actual = target.AddLayout(formVersion, "New Layout");
            Assert.AreEqual(expected, actual, string.Join(Environment.NewLine, expected.GetDifferences(actual)));
        }

        /// <summary>
        /// The save layout test.
        /// </summary>
        [TestMethod]
        public void SaveLayout_NewFormLayout_MatchesExpected()
        {
            var expected = new FormLayout(34) { Name = "Layout1", Instructions = "Do the stuff", Footer = "Mah footer", IsActive = true };

            var layoutPageService = MockRepository.GenerateMock<ILayoutPageService>();

            var layoutPage1 = expected.AddPage("Page 1");
            layoutPage1.Instructions = "Fill this out";
            layoutPage1.Name = "Page 1";
            layoutPage1.ShowHeader = true;

            var layoutPage2 = expected.AddPage("Page 2");
            layoutPage2.Instructions = "Fill this out too";
            layoutPage2.Name = "Page 2";
            layoutPage2.ShowHeader = true;

            var layoutPage3 = expected.AddPage("Page 3");
            layoutPage3.Instructions = "And you thought you were done";
            layoutPage3.Name = "Page 3";
            layoutPage3.ShowHeader = false;

            var pages = new List<LayoutPage> { layoutPage1, layoutPage2, layoutPage3 };

            layoutPageService.Stub(service => service.GetPages(Arg<FormLayout>.Matches(layout => layout == expected))).Return(pages);
            expected.Load(layoutPageService);

            var formLayoutId = 43897;

            var layoutRepository = MockRepository.GenerateMock<IFormLayoutRepository>();
            layoutRepository.Stub(repository => repository.SaveLayout(Arg<FormLayout>.Matches(layout => layout == expected)))
                .Return(null)
                .WhenCalled(
                    invocation =>
                    {
                        var formLayout = invocation.Arguments.OfType<FormLayout>().First();
                        formLayout.SetPropertyValue(layout => layout.FormLayoutId, formLayoutId);

                        foreach (var layoutPage in formLayout.LayoutPages)
                        {
                            layoutPage.SetPropertyValue(page => page.LayoutPageId, NumberGenerator.Next(int.MaxValue));
                        }

                        invocation.ReturnValue = formLayout;
                    });

            var target = new FormLayoutService(layoutRepository);
            var actual = target.SaveLayout(expected);
            Assert.AreSame(expected, actual);
            Assert.AreEqual(formLayoutId, actual.FormLayoutId);

            foreach (var page in actual.LayoutPages)
            {
                Assert.IsTrue(page.LayoutPageId > 0);
            }
        }

        /// <summary>
        /// The save layout test.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void SaveLayout_NewFormLayoutToDatabase_MatchesExpected()
        {
            var entityMapper = new AutoMapperEntityMapper();
            entityMapper.Initialize(
                expression =>
                {
                    expression.AddProfile<UserMappingProfile>();
                    expression.AddProfile<PersonMappingProfile>();
                    expression.AddProfile<FormMappingProfile>();
                    expression.AddProfile<FormVersionMappingProfile>();
                    expression.AddProfile<FormLayoutMappingProfile>();
                    expression.AddProfile<LayoutPageSectionMappingProfile>();
                    expression.AddProfile<LayoutSectionMappingProfile>();
                    expression.AddProfile<FieldPlacementMappingProfile>();
                    expression.AddProfile<UnifiedFieldMappingProfile>();
                    expression.AddProfile<LayoutPageMappingProfile>();
                });

            // Save a form creator and editor.
            var creator = new User("form-creator") { FirstName = "Form", LastName = "Creator" };
            var editor = new User("form-editor") { FirstName = "Form", LastName = "Editor" };

            FormLayout expected;

            using (var provider = new DatabaseRepositoryProvider<FieldsModel>(entityMapper))
            {
                var userRepo = new UserRepository(provider);
                userRepo.Save(creator);
                userRepo.Save(editor);

                var actionContext = MockRepository.GenerateMock<IActionContext>();
                actionContext.Stub(context => context.CurrentPerson).Return(editor);

                var commandProvider = new StructuredSqlCommandProvider(provider);
                var placementRepository = new FieldPlacementRepository(provider);
                var placementService = new FieldPlacementService(placementRepository);
                var pageSectionRepository = new LayoutPageSectionRepository(provider, placementService);
                var pageSectionService = new LayoutPageSectionService(pageSectionRepository);
                var layoutRepository = new FormLayoutRepository(provider, commandProvider, pageSectionService);
                var layoutService = new FormLayoutService(layoutRepository);
                var versionRepository = new FormVersionRepository(provider, layoutService);
                var formVersionService = new FormVersionService(actionContext, versionRepository, layoutService);
                var formRepository = new FormRepository(provider, formVersionService);
                var formService = new FormService(actionContext, formRepository, formVersionService);

                var fieldRepository = new UnifiedFieldRepository(provider, commandProvider);
                var fieldService = new UnifiedFieldService(fieldRepository);
                var unifiedFields = fieldService.SelectAllFields().ToList();

                if (unifiedFields.Count == 0)
                {
                    var systemFields =
                        Generate.SystemFieldSources.SelectMany(type => type.GetNonIndexedProperties())
                            .Where(info => info.PropertyType.IsValueType || info.PropertyType == typeof(string))
                            .Select(Generate.GenerateSystemField);

                    var customFields = Enumerable.Range(1, 1000).Select(Generate.GenerateCustomField);

                    var generatedFields = systemFields.Union(customFields).ToList();

                    var target = new UnifiedFieldService(fieldRepository);
                    unifiedFields.AddRange(target.SaveFields(generatedFields));
                }

                // Create a bunch of random sections and placements.
                var layoutSections =
                    Enumerable.Range(1, 20)
                        .Select(
                            i =>
                                new LayoutSection
                                {
                                    CssStyle = "mah-style",
                                    Name = $"Layout Section {i}",
                                    Instructions = $"Fill this out in {Generate.NumberGenerator.Next(120)} seconds or less or ELSE",
                                    ShowHeader = true
                                })
                        .ToList();

                for (int i = 0; i < 500; i++)
                {
                    var section =
                        layoutSections.ElementAt(Generate.NumberGenerator.Next(layoutSections.Count))
                            .AddPlacement(unifiedFields.ElementAt(Generate.NumberGenerator.Next(unifiedFields.Count)));

                    section.CssStyle = "placement-css";
                }

                var sectionRepository = new LayoutSectionRepository(provider, commandProvider, placementService);
                var sectionService = new LayoutSectionService(sectionRepository);
                var stopwatch = new Stopwatch();

                // Save them all.
                foreach (var section in layoutSections)
                {
                    stopwatch.Restart();
                    var savedSection = sectionService.SaveSection(section);
                    Trace.TraceInformation($"Saved section '{section}' with {section.FieldPlacements.Count()} placements in {stopwatch.Elapsed}");
                    Assert.IsTrue(savedSection.LayoutSectionId > 0);

                    foreach (var placement in savedSection.FieldPlacements)
                    {
                        Assert.IsTrue(placement.FieldPlacementId > 0);
                    }
                }

                var name = "UNIT_TEST:My new form";
                var formVersion = formService.GetByName(name)?.LatestVersion ?? formService.CreateForm(name);

                // Needs to be unique so we can make a new layout.
                expected = formVersionService.AddLayout(formVersion, $"UNIT_TEST:My new layout ({Generate.NumberGenerator.Next(int.MaxValue)})");

                for (int i = 0; i < 5; i++)
                {
                    var page = expected.AddPage($"Page {i}");
                    page.Instructions = "Fill this out";
                    page.ShowHeader = true;

                    for (int j = 0; j < 4; j++)
                    {
                        page.AddSection(layoutSections.ElementAt(Generate.NumberGenerator.Next(layoutSections.Count)));
                    }
                }

                stopwatch.Restart();
                var actual = layoutService.SaveLayout(expected);
                Trace.TraceInformation($"Saved layout in {stopwatch.Elapsed}");
                stopwatch.Stop();
                Assert.IsTrue(actual.FormLayoutId > 0);
                Assert.IsTrue(actual.FormVersionId > 0);

                foreach (var page in actual.LayoutPages)
                {
                    Assert.IsTrue(page.LayoutPageId > 0);

                    foreach (var pageSection in page.LayoutPageSections)
                    {
                        Assert.IsTrue(page.LayoutPageId > 0);
                        Assert.IsTrue(pageSection.LayoutSection.LayoutSectionId > 0);

                        foreach (var placement in pageSection.LayoutSection.FieldPlacements)
                        {
                            Assert.IsTrue(placement.FieldPlacementId > 0);
                        }
                    }
                }
            }

            using (var provider = new DatabaseRepositoryProvider<FieldsModel>(entityMapper))
            {
                var commandProvider = new StructuredSqlCommandProvider(provider);
                var placementRepository = new FieldPlacementRepository(provider);
                var placementService = new FieldPlacementService(placementRepository);
                var pageSectionRepository = new LayoutPageSectionRepository(provider, placementService);
                var pageSectionService = new LayoutPageSectionService(pageSectionRepository);
                var layoutRepository = new FormLayoutRepository(provider, commandProvider, pageSectionService);
                var layoutService = new FormLayoutService(layoutRepository);

                var stopwatch = Stopwatch.StartNew();
                var actual = layoutService.GetLayout(expected.FormLayoutId.GetValueOrDefault());
                Trace.TraceInformation($"Retrieved layout in {stopwatch.Elapsed}.");
                stopwatch.Reset();

                Assert.AreEqual(expected, actual);

                Assert.AreEqual(expected.FormLayoutId, actual.FormLayoutId);
                Assert.AreEqual(expected.FormVersionId, actual.FormVersionId);

                foreach (var page in expected.LayoutPages)
                {
                    var actualPage = actual.LayoutPages.FirstOrDefault(layoutPage => layoutPage.LayoutPageId == page.LayoutPageId);
                    Assert.AreSame(actual, actualPage?.FormLayout);
                    Assert.AreEqual(page, actualPage);

                    foreach (var pageSection in page.LayoutPageSections)
                    {
                        var actualPageSection =
                            actualPage?.LayoutPageSections.FirstOrDefault(x => x.LayoutPageSectionId == pageSection.LayoutPageSectionId);

                        Assert.AreSame(actualPage, actualPageSection?.LayoutPage);
                        Assert.AreEqual(pageSection, actualPageSection);
                        Assert.AreEqual(pageSection.LayoutSectionId, actualPageSection?.LayoutSectionId);
                        Assert.AreEqual(pageSection.LayoutPageId, actualPageSection?.LayoutPageId);

                        foreach (var placement in pageSection.LayoutSection.FieldPlacements)
                        {
                            var actualSection = actualPageSection?.LayoutSection;
                            var actualPlacement = actualSection?.FieldPlacements.FirstOrDefault(x => x.FieldPlacementId == placement.FieldPlacementId);
                            Assert.AreSame(actualSection, actualPlacement?.LayoutSection);
                            Assert.AreEqual(placement, actualPlacement);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The save layout test.
        /// </summary>
        [TestMethod]
        public void SaveLayout_NewFormLayoutWithFormVersionIdEqualZero_ThrowsException()
        {
            var expected = new FormLayout(0) { Name = "Layout1", Instructions = "Do the stuff", Footer = "Mah footer", IsActive = true };

            var layoutPageService = MockRepository.GenerateMock<ILayoutPageService>();

            var layoutPage1 = expected.AddPage("Page 1");
            layoutPage1.Instructions = "Fill this out";
            layoutPage1.Name = "Page 1";
            layoutPage1.ShowHeader = true;

            var layoutPage2 = expected.AddPage("Page 2");
            layoutPage2.Instructions = "Fill this out too";
            layoutPage2.Name = "Page 2";
            layoutPage2.ShowHeader = true;

            var layoutPage3 = expected.AddPage("Page 3");
            layoutPage3.Instructions = "And you thought you were done";
            layoutPage3.Name = "Page 3";
            layoutPage3.ShowHeader = false;

            var pages = new List<LayoutPage> { layoutPage1, layoutPage2, layoutPage3 };

            layoutPageService.Stub(service => service.GetPages(Arg<FormLayout>.Matches(layout => layout == expected))).Return(pages);
            expected.Load(layoutPageService);

            var formLayoutId = 43897;

            var layoutRepository = MockRepository.GenerateMock<IFormLayoutRepository>();
            layoutRepository.Stub(repository => repository.SaveLayout(Arg<FormLayout>.Matches(layout => layout == expected))).Return(null)
                .WhenCalled(
                    invocation =>
                    {
                        var formLayout = invocation.Arguments.OfType<FormLayout>().First();
                        formLayout.SetPropertyValue(layout => layout.FormLayoutId, formLayoutId);

                        foreach (var layoutPage in formLayout.LayoutPages)
                        {
                            layoutPage.SetPropertyValue(page => page.LayoutPageId, NumberGenerator.Next(int.MaxValue));
                        }

                        invocation.ReturnValue = formLayout;
                    });

            var target = new FormLayoutService(layoutRepository);

            try
            {
                var actual = target.SaveLayout(expected);
                Assert.Fail("An exception should have been thrown.");
            }
            catch (BusinessException ex)
            {
                Assert.AreEqual(expected, ex.TargetEntity);
                Assert.IsTrue(ex.Message.Contains(nameof(FormLayout.FormVersionId)));
            }
        }

        /// <summary>
        /// The get layout test.
        /// </summary>
        [TestMethod]
        public void GetLayout_FormLayoutWithPagesById_MatchesExpected()
        {
            var expected = new FormLayout(43897, 132863) { Name = "Layout1", Instructions = "Do the stuff", Footer = "Mah footer", IsActive = true };

            var layoutPageService = MockRepository.GenerateMock<ILayoutPageService>();

            var layoutPage1 = expected.AddPage("Page 1");
            layoutPage1.Instructions = "Fill this out";
            layoutPage1.Name = "Page 1";
            layoutPage1.ShowHeader = true;

            var layoutPage2 = expected.AddPage("Page 2");
            layoutPage2.Instructions = "Fill this out too";
            layoutPage2.Name = "Page 2";
            layoutPage2.ShowHeader = true;

            var layoutPage3 = expected.AddPage("Page 3");
            layoutPage3.Instructions = "And you thought you were done";
            layoutPage3.Name = "Page 3";
            layoutPage3.ShowHeader = false;

            var pages = new List<LayoutPage> { layoutPage1, layoutPage2, layoutPage3 };

            layoutPageService.Stub(service => service.GetPages(Arg<FormLayout>.Matches(layout => layout == expected))).Return(pages);
            expected.Load(layoutPageService);

            var layoutRepository = MockRepository.GenerateMock<IFormLayoutRepository>();
            var formLayoutId = expected.FormLayoutId.GetValueOrDefault();
            layoutRepository.Stub(repository => repository.GetLayout(Arg<int>.Matches(i => i == formLayoutId)))
                .Return(expected);

            var target = new FormLayoutService(layoutRepository);
            var actual = target.GetLayout(formLayoutId);

            Assert.AreSame(expected, actual);
            CollectionAssert.AreEqual(pages, actual.LayoutPages.ToList());
        }

        /// <summary>
        /// The get layout test.
        /// </summary>
        [TestMethod]
        public void GetLayout_FormLayoutWithPagesByIdEqualToZero_ThrowsException()
        {
            var expected = new FormLayout(43897, 0) { Name = "Layout1", Instructions = "Do the stuff", Footer = "Mah footer", IsActive = true };

            var layoutPageService = MockRepository.GenerateMock<ILayoutPageService>();

            var layoutPage1 = expected.AddPage("Page 1");
            layoutPage1.Instructions = "Fill this out";
            layoutPage1.Name = "Page 1";
            layoutPage1.ShowHeader = true;

            var layoutPage2 = expected.AddPage("Page 2");
            layoutPage2.Instructions = "Fill this out too";
            layoutPage2.Name = "Page 2";
            layoutPage2.ShowHeader = true;

            var layoutPage3 = expected.AddPage("Page 3");
            layoutPage3.Instructions = "And you thought you were done";
            layoutPage3.Name = "Page 3";
            layoutPage3.ShowHeader = false;

            var pages = new List<LayoutPage> { layoutPage1, layoutPage2, layoutPage3 };

            layoutPageService.Stub(service => service.GetPages(Arg<FormLayout>.Matches(layout => layout == expected))).Return(pages);
            expected.Load(layoutPageService);

            var layoutRepository = MockRepository.GenerateMock<IFormLayoutRepository>();
            var formLayoutId = expected.FormLayoutId.GetValueOrDefault();
            layoutRepository.Stub(repository => repository.GetLayout(Arg<int>.Matches(i => i == formLayoutId)))
                .Return(expected);

            var target = new FormLayoutService(layoutRepository);

            try
            {
                var actual = target.GetLayout(formLayoutId);
                Assert.Fail("An exception should have been thrown.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.AreEqual(0, ex.ActualValue);
                Assert.AreEqual("id", ex.ParamName);
                Assert.IsTrue(ex.Message.Contains(FieldsMessages.IdValueLessThanOne));
            }
        }
    }
}
