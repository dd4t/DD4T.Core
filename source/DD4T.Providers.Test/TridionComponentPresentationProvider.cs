using System;
using System.Collections.Generic;
using DD4T.ContentModel.Querying;
using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Contracts.Serializing;
using DD4T.ContentModel;

namespace DD4T.Providers.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TridionComponentPresentationProvider : BaseProvider, IComponentPresentationProvider
    {

        public ISerializerService SerializerService
        {
            get;
            set;
        }

        public string GetContent(string uri, string templateUri = "")
        {
            Schema schema = new Schema();
            schema.Title = Randomizer.AnyString(10);
            Component component = new Component();
            component.Title = Randomizer.AnyString(30);
            component.Id = Randomizer.AnyUri(16);
            component.Schema = schema;

            Field field1 = Randomizer.AnyTextField(6, 120, true);
            Field field2 = Randomizer.AnyTextField(8, 40, false);

            FieldSet fieldSet = new FieldSet();
            fieldSet.Add(field1.Name, field1);
            fieldSet.Add(field2.Name, field2);
            component.Fields = fieldSet;

            if (templateUri == "componentlink")
            {
                CustomizeCompomentForComponentLink(component);
            }
            if (templateUri == "embedded")
            {
                CustomizeCompomentForEmbeddedField(component);
            }
            if (templateUri == "keyword")
            {
                CustomizeCompomentForKeywordField(component);
            }
            if(templateUri == "componentIgnoreCase")
            {
                CustomizeCompomentForComponentLinkIgnoreCase(component);
            }
           
            if (uri == "component")
            {
                return SerializerService.Serialize<Component>(component);
            }

            ComponentTemplate ct = new ComponentTemplate();
            ct.Title = Randomizer.AnyString(20);
            Field fieldView = new Field();
            fieldView.Name = "view";
            fieldView.Values.Add("DefaultComponentView");
            ct.MetadataFields = new FieldSet();
            ct.MetadataFields.Add(fieldView.Name, fieldView);

            ComponentPresentation cp = new ComponentPresentation();
            cp.Component = component;
            cp.ComponentTemplate = ct;

            Condition condition = new KeywordCondition() { Keyword = new Keyword() { Id = "tcm:2-123-1024", Key = "test", Title = "Test" }, Operator = NumericalConditionOperator.Equals, Value = 1 };
            cp.Conditions = new List<Condition>() { condition };

            return SerializerService.Serialize<ComponentPresentation>(cp);
        }

        private void CustomizeCompomentForKeywordField(Component component)
        {
            component.Schema.RootElementName = "hasKeyword";
            Field headingField = new Field() { Name = "heading", Values = new List<string> { "some heading" } };

            FieldSet metadataFields = new FieldSet();
            metadataFields.Add(headingField.Name, headingField);


            Field keywordField = new Field()
            {
                Name = "keyword",
                KeywordValues = new List<Keyword>() 
                {
                    new Keyword()
                    {
                        MetadataFields = metadataFields,
                        Id = Randomizer.AnyUri(1024),
                        Title = Randomizer.AnyString(33),
                        Description = Randomizer.AnyString(33)
                    }                        
                }
            };
            component.Fields.Add(keywordField.Name, keywordField);
        }

        private void CustomizeCompomentForEmbeddedField(Component component)
        {
            component.Schema.RootElementName = "rootEmbedding";
            Field headingField = new Field() { Name = "heading", Values = new List<string> { "some heading" } };
            FieldSet embeddedFields = new FieldSet();
            embeddedFields.Add(headingField.Name, headingField);
            Field embeddedField = new Field()
            {
                Name = "embedded",
                EmbeddedSchema = new Schema()
                {
                    Title = "EmbeddedSchema",
                    RootElementName = "embeddedRoot",
                    Id = Randomizer.AnyUri(8)
                },
                EmbeddedValues = new List<FieldSet>() 
                {
                    embeddedFields
                }
            };
            component.Fields.Add(embeddedField.Name, embeddedField);
        }
        private void CustomizeCompomentForComponentLink(Component component)
        {
            component.Schema.RootElementName = "rootA";
            Field headingField = new Field() { Name = "heading", Values = new List<string> { "some heading" } };
            FieldSet fieldsForLinkedComponent = new FieldSet();
            fieldsForLinkedComponent.Add(headingField.Name, headingField);
            Field linkField = new Field()
            {
                Name = "link",
                LinkedComponentValues = new List<Component> 
                {
                    new Component() 
                    {
                        Title = Randomizer.AnyString(16),
                        Id = Randomizer.AnyUri(16),
                        Schema = new Schema()
                        {
                            Title = Randomizer.AnyString(10),
                            RootElementName = "rootB"
                        },
                        Fields = fieldsForLinkedComponent
                    }
                }
            };
            component.Fields.Add(linkField.Name, linkField);
        }

        private void CustomizeCompomentForComponentLinkIgnoreCase(Component component)
        {
            component.Schema.RootElementName = "RootA";
            Field headingField = new Field() { Name = "Heading", Values = new List<string> { "some heading" } };
            FieldSet fieldsForLinkedComponent = new FieldSet();
            fieldsForLinkedComponent.Add(headingField.Name, headingField);
            Field linkField = new Field()
            {
                Name = "Link",
                LinkedComponentValues = new List<Component>
                {
                    new Component()
                    {
                        Title = Randomizer.AnyString(16),
                        Id = Randomizer.AnyUri(16),
                        Schema = new Schema()
                        {
                            Title = Randomizer.AnyString(10),
                            RootElementName = "RootB"
                        },
                        Fields = fieldsForLinkedComponent
                    }
                }
            };
            component.Fields.Add(linkField.Name, linkField);
        }


        /// <summary>
        /// Returns the Component contents which could be found. Components that couldn't be found don't appear in the list. 
        /// </summary>
        /// <param name="componentUris"></param>
        /// <returns></returns>
        public List<string> GetContentMultiple(string[] componentUris)
        {
            string testCP1 = GetContent("");
            string testCP2 = GetContent("");
            return new List<string>() { testCP1, testCP2 };
        }

        public IList<string> FindComponents(IQuery query)
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastPublishedDate(string uri)
        {
            return DateTime.Now;
        }
    }
}
