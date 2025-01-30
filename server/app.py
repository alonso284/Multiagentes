from flask import Flask, send_file
import os

app = Flask(__name__)

# Set the path to your CSV file
CSV_FILE_PATH = 'steps.csv'  # Replace with the path to your CSV file

@app.route('/download-csv', methods=['GET'])
def download_csv():
    # Check if the file exists
    if os.path.exists(CSV_FILE_PATH):
        return send_file(CSV_FILE_PATH, as_attachment=True)
    else:
        return "File not found", 404

if __name__ == '__main__':
    # Run the app on your local machine (default is localhost:5000)
    app.run(debug=True)
