import json
import re


class SingleQuoteDecoder(json.JSONDecoder):
    def decode(self, s):
        return super().decode(s.replace("'", '"'))


def read_json_with_single_quotes(file_path, encoding='utf-8', **kwargs):
    with open(file_path, 'r', encoding=encoding) as file:
        json_string = file.read()

    # Use the custom decoder to convert single quotes to double quotes
    try:
        tickers = []
        parsed_json = json.loads(json_string, cls=SingleQuoteDecoder)
        for key in parsed_json:
            tickers.append(key)

        print(tickers)
        filtered_tickers = [ticker for ticker in tickers if 'VUAA' in ticker]

        # Output the result
        print(filtered_tickers)
    except Exception as ex:
        print(ex.__str__())
        error_message = ex.__str__()
        # Extract the column number from the error message
        column_number = int(re.search(r'column (\d+)', error_message).group(1))
        print(column_number)
        # Define the input/output file path (since we're modifying the same file)
        file_path = "yhallsym.json"
        # Read the file content as text
        with open(file_path, 'r', encoding='utf-8') as file:
            file_content = file.read()
        # Find the character at the specified column number
        target_char = file_content[column_number - 3]  # Column is 1-indexed
        target_char2 = file_content[column_number - 2]  # Column is 1-indexed
        print(target_char + target_char2)
        # Perform the replacement
        if target_char + target_char2 == "' ":
            # Remove the single quote
            modified_content = file_content[:column_number - 3] + ' ' + file_content[column_number - 1:]
        elif target_char + target_char2 == ": ":
            # Add single quotes around the space
            modified_content = file_content[:column_number - 3] + " '" + file_content[column_number - 1:]
        elif target_char2 == "'":
            modified_content = file_content[:column_number - 2] + file_content[column_number - 1:]
        else:
            # If it's neither, leave it unchanged
            modified_content = file_content

        print(modified_content[column_number - 10:column_number + 10])
        # Save the modified content back to the same file
        with open('yhallsym.json', 'w', encoding='utf-8') as file:
            file.write(modified_content)
        print(f"Modified content saved to {file_path}")


# Usage
try:
    df = read_json_with_single_quotes('yhallsym.json', encoding='utf-8')
    print(df)
except UnicodeDecodeError:
    print("UTF-8 encoding failed. Trying with 'latin-1' encoding.")
    df = read_json_with_single_quotes('yhallsym.json', encoding='latin-1')
    print(df)
