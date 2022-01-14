# webex-attendance
Uses Emgu OpenCV and C# Windows Forms to do text recognition for attendance taking in webex

#### Models Links
https://github.com/opencv/opencv/blob/4.x/doc/tutorials/dnn/dnn_text_spotting/dnn_text_spotting.markdown
uses CRNN_VGG_BiLSTM_CTC.onnx and DB_IC15_resnet18.onnx, sourced from opencv's documentation / tutorial


#### Usage
1. Edit NAMELIST.txt with the names you want to detect
2. Edit FILTER.txt for any text you want to ignore
3. Download models and place in models folder
4. Run the exe
5. Place window over webex participant (Make size participant list to cover completely)
6. Press DETECT button
7. Wait for results