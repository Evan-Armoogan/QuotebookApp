import datetime
import time
import random
import os
import json
from oauth2client.service_account import ServiceAccountCredentials
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from keep_alive import keep_alive

GServAcc = json.loads(os.environ['GServAcc'])
SpreadsheetID = os.environ['SpreadsheetID']

QUOTES_HISTORY = 30
TZ_DELTA = -4
SCOPES = 'https://www.googleapis.com/auth/spreadsheets'


def get_credentials():
  creds = ServiceAccountCredentials.from_json_keyfile_dict(GServAcc,
                                                           scopes=SCOPES)
  return creds


def get_quotes(creds):
  try:
    service = build('sheets', 'v4', credentials=creds)

    spreadsheet_id = SpreadsheetID
    range_name = "Quotes!A2:D"

    result = service.spreadsheets().values().get(spreadsheetId=spreadsheet_id,
                                                 range=range_name).execute()
    rows = result.get('values', [])
    return rows
  except HttpError as error:
    print(f"An error occurred: {error}")
    return error


def update_qotd(creds, vals):
  try:
    service = build('sheets', 'v4', credentials=creds)

    value_input_option = "USER_ENTERED"
    range_name = "QOTD!A2:D2"
    spreadsheet_id = SpreadsheetID

    values = [vals]
    body = {'values': values}

    result = service.spreadsheets().values().update(
        spreadsheetId=spreadsheet_id,
        range=range_name,
        valueInputOption=value_input_option,
        body=body).execute()
    print(f"{result.get('updatedCells')} cells updated.")
    return result
  except HttpError as error:
    print(f"An error occurred: {error}")
    return error


def process_choose_quote(creds):
  quotes = get_quotes(creds)
  if type(quotes) == HttpError:
    return False

  try:
    lines = []
    with open('recent_quotes.txt', 'r') as f:
      lines = f.readlines()

    while True:
      good_quote = True
      rand_quote = random.randrange(0, len(quotes))
      for line in lines:
        if int(line) == rand_quote:
          good_quote = False
      if good_quote:
        break

    with open('recent_quotes.txt', 'r') as f:
      data = f.read().splitlines(True)
      if len(lines) >= QUOTES_HISTORY:
        # remove first line (only store history of QUOTES_HISTORY quotes)
        data = data[1:]
      x = ''
      if len(lines) > 0:
        x = '\n'
      x += str(rand_quote)
      data.append(x)
    with open('recent_quotes.txt', 'w') as f:
      for line in data:
        f.writelines(line)

    # Quote is good, now use it
    quote_vals = quotes[rand_quote]
    update_qotd(creds, quote_vals)

    return True
  except:
    return False


def write_last_quote_time():
  # do this when we change qotd
  this_time = datetime.datetime.now() + datetime.timedelta(hours=TZ_DELTA)
  this_date = this_time.date()
  this_dt = datetime.datetime.combine(this_date, datetime.datetime.min.time())
  last_updated = time.mktime(this_dt.timetuple())
  with open('last_qotd_timestamp.txt', 'w') as f:
    f.write(str(last_updated))
  return last_updated


def main():
  creds = get_credentials()

  with open('last_qotd_timestamp.txt', 'r') as f:
    try:
      last_updated = float(f.readlines()[0])
    except:
      last_updated = 0

  while True:
    this_time = datetime.datetime.now() + datetime.timedelta(hours=TZ_DELTA)
    if (time.mktime(this_time.timetuple()) - last_updated) > 24 * 60 * 60:
      ret = process_choose_quote(creds)
      if ret:
        last_updated = write_last_quote_time()


if __name__ == "__main__":
  keep_alive()
  main()
