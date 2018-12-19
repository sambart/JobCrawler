# -*- coding: utf-8 -*-

import requests
from bs4 import BeautifulSoup

# HTTP GET Request
req = requests.get('http://www.jobkorea.co.kr/recruit/joblist?menucode=duty&dutyCtgr=10016')
# HTML 소스 가져오기
html = req.text
# BeautifulSoup으로 html소스를 python객체로 변환하기
# 첫 인자는 html소스코드, 두 번째 인자는 어떤 parser를 이용할지 명시.
# 이 글에서는 Python 내장 html.parser를 이용했다.
soup = BeautifulSoup(html, 'html.parser')
#print(soup)

my_titles = soup.select(
    'tr > td.tplCo > a'
    )
for title in my_titles:
    # Tag안의 텍스트
    print(title.text)
    # Tag의 속성을 가져오기(ex: href속성)
    print(title.get('href'))