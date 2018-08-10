# HelioViewer Image Crawler

This program was created as part of the bachelor's thesis by Sandro Schwager and Fabio Strappazzon.

## Usage

This Tool downloads a sequence of JPEG 2000 images from api.helioviewer.org. SourceID is hardcoded to be 10 (AIA 171). The images will be saved in the current working directory.

**Usage:** HelioViewerImageCrawler startDate imageCount timeInterval  
**startDate:** Date of first Image  
**imageCount:** how many images  
**timeInterval:** [x1|x4|x16|x64|x256] x1 downloads every image (interval of 36 seconds), x4 every fourth and so on.  
**example:** HelioViewerImageCrawler 2018-1-1 3 x4



## Dependencies
* Newtonsoft.Json
* Unirest-API