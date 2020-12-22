using AlignNamespacesExtension.XUnit.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace AlignNamespacesExtension.XUnit.MockData
{
    public static class FolderNodeMockData
    {

        public static FolderNode Instance = new FolderNode()
        {
            Name = "C:",
            Files = new Dictionary<string, string>() { },
            Folders = new List<FolderNode>()
            {
                new FolderNode(){
                    Name = "Program Files (x86)",
                    Files = new Dictionary<string, string>() { },
                    Folders = new List<FolderNode>(){
                        new FolderNode(){
                             Name = "one",
                             Files = new Dictionary<string, string>() {
                                 { "one.cs", ""},
                                 { "two.cs", ""},
                                 { "three.cs", ""},
                                 { "three.cx", ""},
                                 { "three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        },
                        new FolderNode(){
                             Name = "two",
                             Files = new Dictionary<string, string>() {
                                 { "one.cs", ""},
                                 { "two.cs", ""},
                                 { "three.cs", ""},
                                 { "three.cx", ""},
                                 { "three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        },
                        new FolderNode(){
                             Name = "three",
                             Files = new Dictionary<string, string>() {
                                 { "one.cs", ""},
                                 { "two.cs", ""},
                                 { "three.cs", ""},
                                 { "three.cx", ""},
                                 { "three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        }
                    }
                },
                new FolderNode(){
                    Name = "two",
                    Files = new Dictionary<string, string>() { },
                    Folders = new List<FolderNode>(){
                        new FolderNode(){
                             Name = "one",
                             Files = new Dictionary<string, string>() {
                                 { "one.cs", ""},
                                 { "two.cs", ""},
                                 { "three.cs", ""},
                                 { "three.cx", ""},
                                 { "three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        },
                        new FolderNode(){
                             Name = "two",
                             Files = new Dictionary<string, string>() {
                                 { "one.cs", ""},
                                 { "two.cs", ""},
                                 { "three.cs", ""},
                                 { "three.cx", ""},
                                 { "three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        },
                        new FolderNode(){
                             Name = "three",
                             Files = new Dictionary<string, string>() {
                                 { "one.cs", ""},
                                 { "two.cs", ""},
                                 { "three.cs", ""},
                                 { "three.cx", ""},
                                 { "three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        }
                    }
                },
                new FolderNode(){
                    Name = "three",
                    Files = new Dictionary<string, string>() { },
                    Folders = new List<FolderNode>(){
                        new FolderNode(){
                             Name = "one",
                             Files = new Dictionary<string, string>() {
                                 { "one_one.cs", "testtext"},
                                 { "one_two.cs", ""},
                                 { "one_three.cs", ""},
                                 { "one_three.cx", ""},
                                 { "one_three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        },
                        new FolderNode(){
                             Name = "two",
                             Files = new Dictionary<string, string>() {
                                 { "two_one.cs", "testtext2"},
                                 { "two_two.cs", ""},
                                 { "two_three.cs", ""},
                                 { "two_three.cx", ""},
                                 { "two_three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        },
                        new FolderNode(){
                             Name = "three",
                             Files = new Dictionary<string, string>() {
                                 { "three_one.cs", "testtext3"},
                                 { "three_two.cs", ""},
                                 { "three_three.cs", ""},
                                 { "three_three.cx", ""},
                                 { "three_three.r.g", ""}
                             },
                             Folders = new List<FolderNode>(){ }
                        }
                    }
                }
            },
        };

        public static async Task<FolderNode> LoadFromSystem(string filePath)
        {
            var files = Directory.GetFiles(filePath);
            var directories = Directory.GetDirectories(filePath);

            var result = new FolderNode()
            {
                Name = filePath.Split(Path.DirectorySeparatorChar).LastOrDefault(),
                Folders = new List<FolderNode>(),
                Files = new Dictionary<string, string>()
            };

            Parallel.ForEach(files, (f) =>
            {
                result.Files[f] = File.ReadAllText(f);
            });

            Parallel.ForEach(directories, async (d) =>
            {
                var childNode = await LoadFromSystem(d);
                result.Folders.Add(childNode);
            });

            return result;
        }
    }
}
