using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using QMTGroup.Web.ViewModel;

namespace QMTGroup.Web.Service
{
    public class SequencerStorageService
    {
        private string _folderLocation;
        private readonly string _defaultSequence = "### job.position: 216, 436\nacquisition_job:\n  stage: acquisition\n  script: echo \"Acquire\"\n\n### job.position: 604, 432\nprepare_job:\n  stage: build\n  script: echo \"Prepare environment\"\n\n### job.position: 384, 802\ntest_job:\n  stage: test\n  needs:\n    - acquisition_job\n    - prepare_job\n  script: echo \"Test everything\"\n\n### job.position: 640, 1170\ndeploy_job:\n  stage: deploy\n  needs:\n    - test_job\n    - prepare_job\n  script: echo \"Deploy\"\n";

        private string _defaultSavedSequence;

        private readonly string[] _blackList = [ "_default" ];

        public SequencerStorageService(IConfiguration configuration)
        {
            _folderLocation = configuration["SequencerFolder"] ?? throw new ArgumentNullException("'SequencerFolder' don't exist in the configuration file.");
            Path.GetFullPath(_folderLocation);
            if (!Path.Exists(_folderLocation))
                Directory.CreateDirectory(_folderLocation);

            _getOrCreateDefaultSequence();
        }

        private string _getOrCreateDefaultSequence()
        {
            var defaultFilePath = Path.Combine(_folderLocation, "_default.yaml");
            if (!File.Exists(defaultFilePath))
                File.WriteAllText(defaultFilePath, _defaultSequence);

            return File.ReadAllText(defaultFilePath);
        }

        public IEnumerable<SequenceStorageItemViewModel> GetAllSequences()
        {
            var files = Directory.GetFiles(_folderLocation, "*.yaml");

            return files
                .Where(x => !_blackList.Contains(Path.GetFileNameWithoutExtension(x)))
                .Select(x =>
                {
                    var fi = new FileInfo(x);

                    return new SequenceStorageItemViewModel()
                    {
                        Title = Path.GetFileNameWithoutExtension(x),
                        Description = string.Empty,
                        CreationDate = fi.CreationTime,
                        LastUpdateDate = fi.LastWriteTime,
                        ResourceLocation = x,
                    };
                });
        }

        public bool CreateSequence(string sequenceName)
        {
            string filePath;
            try
            {
                filePath = Path.Combine(_folderLocation, sequenceName + ".yaml");
            }catch(Exception)
            {
                return false;
            }

            if (File.Exists(filePath))
                return false;

            string defaultSequence = _getOrCreateDefaultSequence();

            File.WriteAllText(filePath, defaultSequence);

            return true;
        }

        public bool Delete(string sequenceName)
        {
            string filePath;

            try
            {
                filePath = Path.Combine(_folderLocation, sequenceName + ".yaml");
            }
            catch (Exception)
            {
                return false;
            }

            if (!File.Exists(filePath))
                return true;

            FileSystem.DeleteFile(
                filePath,
                UIOption.OnlyErrorDialogs,
                RecycleOption.SendToRecycleBin);

            return true;
        }

        public bool Exists(string sequenceName)
        {
            string filePath;
            try
            {
                filePath = Path.Combine(_folderLocation, sequenceName + ".yaml");
            }
            catch (Exception)
            {
                return false;
            }

            return File.Exists(filePath);
        }

        public string GetCode(string sequenceName)
        {
            string filePath;
            try
            {
                filePath = Path.Combine(_folderLocation, sequenceName + ".yaml");
            }
            catch (Exception)
            {
                return string.Empty;
            }

            if (!File.Exists(filePath))
                return string.Empty;

            return File.ReadAllText(filePath);
        }

        public string GetDirectory()
            => _folderLocation;
    }
}
