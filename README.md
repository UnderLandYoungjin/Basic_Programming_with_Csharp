# Basic_Programming_with_Csharp
폴리텍하이테크과정 기본프로그래밍 강의
<img width="786" height="410" alt="image" src="https://github.com/user-attachments/assets/25a36bec-a4e1-4ca7-bec3-9233424ad762" />
[profile_huh_youngjin.html](https://github.com/user-attachments/files/25541156/profile_huh_youngjin.html)
<!DOCTYPE html>
<html lang="ko">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>허영진 강사 프로필</title>
<link href="https://fonts.googleapis.com/css2?family=Noto+Sans+KR:wght@300;400;500;700;900&family=Space+Mono:wght@400;700&display=swap" rel="stylesheet">
<style>
  :root {
    --bg: #0a0e1a;
    --surface: #111827;
    --surface2: #1c2538;
    --accent: #00d4ff;
    --accent2: #7c3aed;
    --accent3: #10b981;
    --text: #e2e8f0;
    --text-muted: #64748b;
    --border: rgba(0,212,255,0.15);
  }

  * { margin: 0; padding: 0; box-sizing: border-box; }

  body {
    background: var(--bg);
    color: var(--text);
    font-family: 'Noto Sans KR', sans-serif;
    min-height: 100vh;
    overflow-x: hidden;
  }

  /* Grid background */
  body::before {
    content: '';
    position: fixed;
    inset: 0;
    background-image:
      linear-gradient(rgba(0,212,255,0.03) 1px, transparent 1px),
      linear-gradient(90deg, rgba(0,212,255,0.03) 1px, transparent 1px);
    background-size: 60px 60px;
    pointer-events: none;
    z-index: 0;
  }

  .container {
    max-width: 900px;
    margin: 0 auto;
    padding: 60px 24px;
    position: relative;
    z-index: 1;
  }

  /* Hero Section */
  .hero {
    display: grid;
    grid-template-columns: 1fr auto;
    gap: 40px;
    align-items: start;
    margin-bottom: 60px;
    padding: 48px;
    background: var(--surface);
    border: 1px solid var(--border);
    border-radius: 2px;
    position: relative;
    overflow: hidden;
  }

  .hero::before {
    content: '';
    position: absolute;
    top: 0; left: 0; right: 0;
    height: 3px;
    background: linear-gradient(90deg, var(--accent), var(--accent2), var(--accent3));
  }

  .hero::after {
    content: 'PROFILE';
    position: absolute;
    top: 16px; right: 20px;
    font-family: 'Space Mono', monospace;
    font-size: 10px;
    letter-spacing: 4px;
    color: var(--text-muted);
    opacity: 0.5;
  }

  .name-block { }

  .name-ko {
    font-size: clamp(2.5rem, 6vw, 4rem);
    font-weight: 900;
    line-height: 1;
    letter-spacing: -2px;
    background: linear-gradient(135deg, #fff 0%, var(--accent) 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
    margin-bottom: 8px;
  }

  .name-en {
    font-family: 'Space Mono', monospace;
    font-size: 0.9rem;
    color: var(--accent);
    letter-spacing: 3px;
    text-transform: uppercase;
    margin-bottom: 24px;
  }

  .role-badge {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    background: rgba(0,212,255,0.08);
    border: 1px solid rgba(0,212,255,0.25);
    color: var(--accent);
    font-size: 0.8rem;
    font-weight: 500;
    padding: 6px 14px;
    letter-spacing: 1px;
  }

  .role-badge::before {
    content: '';
    width: 6px; height: 6px;
    background: var(--accent);
    border-radius: 50%;
    animation: pulse 2s infinite;
  }

  @keyframes pulse {
    0%, 100% { opacity: 1; transform: scale(1); }
    50% { opacity: 0.4; transform: scale(0.8); }
  }

  .avatar-box {
    width: 120px;
    height: 120px;
    background: linear-gradient(135deg, var(--accent2), var(--accent));
    border-radius: 2px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 3rem;
    flex-shrink: 0;
    position: relative;
  }

  .avatar-box::after {
    content: '';
    position: absolute;
    inset: -4px;
    border: 1px solid rgba(0,212,255,0.3);
    border-radius: 2px;
  }

  /* Sections */
  .section {
    margin-bottom: 32px;
    background: var(--surface);
    border: 1px solid var(--border);
    border-radius: 2px;
    overflow: hidden;
    animation: fadeUp 0.5s ease both;
  }

  @keyframes fadeUp {
    from { opacity: 0; transform: translateY(20px); }
    to { opacity: 1; transform: translateY(0); }
  }

  .section:nth-child(2) { animation-delay: 0.1s; }
  .section:nth-child(3) { animation-delay: 0.2s; }
  .section:nth-child(4) { animation-delay: 0.3s; }
  .section:nth-child(5) { animation-delay: 0.4s; }

  .section-header {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 18px 28px;
    border-bottom: 1px solid var(--border);
    background: rgba(255,255,255,0.02);
  }

  .section-icon {
    font-size: 1.1rem;
  }

  .section-title {
    font-family: 'Space Mono', monospace;
    font-size: 0.75rem;
    font-weight: 700;
    letter-spacing: 3px;
    text-transform: uppercase;
    color: var(--accent);
  }

  .section-body {
    padding: 24px 28px;
  }

  /* Education */
  .edu-item {
    display: flex;
    gap: 16px;
    align-items: flex-start;
  }

  .edu-dot {
    width: 10px; height: 10px;
    background: var(--accent);
    border-radius: 50%;
    margin-top: 6px;
    flex-shrink: 0;
    box-shadow: 0 0 12px var(--accent);
  }

  .edu-text {
    font-size: 1rem;
    font-weight: 500;
    line-height: 1.6;
  }

  .edu-sub {
    font-size: 0.82rem;
    color: var(--text-muted);
    margin-top: 4px;
    line-height: 1.5;
  }

  /* Certs */
  .certs-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
  }

  .cert-tag {
    background: rgba(124,58,237,0.12);
    border: 1px solid rgba(124,58,237,0.3);
    color: #a78bfa;
    padding: 8px 16px;
    font-size: 0.85rem;
    font-weight: 500;
    border-radius: 2px;
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .cert-tag::before {
    content: '◆';
    font-size: 0.5rem;
    color: var(--accent2);
  }

  /* Career */
  .career-list {
    display: flex;
    flex-direction: column;
    gap: 0;
  }

  .career-item {
    display: flex;
    gap: 20px;
    align-items: flex-start;
    padding: 16px 0;
    border-bottom: 1px solid rgba(255,255,255,0.04);
  }

  .career-item:last-child { border-bottom: none; }

  .career-num {
    font-family: 'Space Mono', monospace;
    font-size: 0.7rem;
    color: var(--text-muted);
    padding-top: 3px;
    min-width: 24px;
  }

  .career-text {
    font-size: 0.95rem;
    line-height: 1.5;
    color: var(--text);
  }

  /* Skills */
  .skills-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 16px;
  }

  .skill-card {
    background: var(--surface2);
    border: 1px solid rgba(255,255,255,0.06);
    padding: 18px 20px;
    border-radius: 2px;
    transition: border-color 0.2s, transform 0.2s;
    cursor: default;
  }

  .skill-card:hover {
    border-color: rgba(0,212,255,0.3);
    transform: translateY(-2px);
  }

  .skill-category {
    font-family: 'Space Mono', monospace;
    font-size: 0.65rem;
    letter-spacing: 2px;
    text-transform: uppercase;
    color: var(--accent3);
    margin-bottom: 10px;
  }

  .skill-items {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
  }

  .skill-chip {
    background: rgba(16,185,129,0.08);
    border: 1px solid rgba(16,185,129,0.2);
    color: #6ee7b7;
    font-size: 0.78rem;
    padding: 3px 10px;
    border-radius: 2px;
  }

  /* footer */
  .footer {
    text-align: center;
    margin-top: 48px;
    font-family: 'Space Mono', monospace;
    font-size: 0.7rem;
    color: var(--text-muted);
    letter-spacing: 2px;
    opacity: 0.5;
  }

  /* scan line effect */
  .hero::before {
    content: '';
    position: absolute;
    top: 0; left: 0; right: 0;
    height: 3px;
    background: linear-gradient(90deg, var(--accent), var(--accent2), var(--accent3));
    z-index: 1;
  }

  @media (max-width: 600px) {
    .hero {
      grid-template-columns: 1fr;
      padding: 32px 24px;
    }
    .avatar-box { display: none; }
    .skills-grid { grid-template-columns: 1fr; }
  }
</style>
</head>
<body>
<div class="container">

  <!-- Hero -->
  <div class="hero" style="animation: fadeUp 0.4s ease both;">
    <div class="name-block">
      <div class="name-ko">허영진</div>
      <div class="name-en">YoungJin Huh</div>
      <div class="role-badge">AI · 임베디드 · 제어 전문가</div>
    </div>
    <div class="avatar-box">🤖</div>
  </div>

  <!-- 학력 -->
  <div class="section">
    <div class="section-header">
      <span class="section-icon">🎓</span>
      <span class="section-title">Education</span>
    </div>
    <div class="section-body">
      <div class="edu-item">
        <div class="edu-dot"></div>
        <div>
          <div class="edu-text">부경대학교 지능로봇공학 <strong>석사</strong></div>
          <div class="edu-sub">AI Vision 기반 졸음 인식 및 객체 탐지 시스템 연구</div>
        </div>
      </div>
    </div>
  </div>

  <!-- 자격 -->
  <div class="section">
    <div class="section-header">
      <span class="section-icon">📜</span>
      <span class="section-title">Certifications</span>
    </div>
    <div class="section-body">
      <div class="certs-grid">
        <div class="cert-tag">전기기능장</div>
        <div class="cert-tag">정보처리기사</div>
        <div class="cert-tag">FDM 3D프린팅 관련자격증</div>
      </div>
    </div>
  </div>

  <!-- 경력 -->
  <div class="section">
    <div class="section-header">
      <span class="section-icon">💼</span>
      <span class="section-title">Career</span>
    </div>
    <div class="section-body">
      <div class="career-list">
        <div class="career-item">
          <span class="career-num">01</span>
          <span class="career-text">UnderLand 제품개발 및 소싱</span>
        </div>
        <div class="career-item">
          <span class="career-num">02</span>
          <span class="career-text">산업용 제어반 및 PLC 시스템 실무</span>
        </div>
        <div class="career-item">
          <span class="career-num">03</span>
          <span class="career-text">한·중 기술무역 및 기계 부품 소싱 경험</span>
        </div>
        <div class="career-item">
          <span class="career-num">04</span>
          <span class="career-text">AI Vision 기반 산업 모니터링 시스템 개발</span>
        </div>
      </div>
    </div>
  </div>

  <!-- 기술 -->
  <div class="section">
    <div class="section-header">
      <span class="section-icon">🔧</span>
      <span class="section-title">Technical Skills</span>
    </div>
    <div class="section-body">
      <div class="skills-grid">
        <div class="skill-card">
          <div class="skill-category">AI Vision</div>
          <div class="skill-items">
            <span class="skill-chip">YOLO</span>
            <span class="skill-chip">MediaPipe</span>
            <span class="skill-chip">OpenCV</span>
          </div>
        </div>
        <div class="skill-card">
          <div class="skill-category">Languages</div>
          <div class="skill-items">
            <span class="skill-chip">Python</span>
            <span class="skill-chip">C#</span>
            <span class="skill-chip">C/C++</span>
          </div>
        </div>
        <div class="skill-card">
          <div class="skill-category">Control Systems</div>
          <div class="skill-items">
            <span class="skill-chip">PLC 제어</span>
            <span class="skill-chip">LS XBC 계열</span>
          </div>
        </div>
        <div class="skill-card">
          <div class="skill-category">Embedded</div>
          <div class="skill-items">
            <span class="skill-chip">AVR</span>
            <span class="skill-chip">Linux 보드</span>
          </div>
        </div>
        <div class="skill-card">
          <div class="skill-category">Web / Dashboard</div>
          <div class="skill-items">
            <span class="skill-chip">Flutter</span>
            <span class="skill-chip">Flask</span>
            <span class="skill-chip">산업 대시보드</span>
          </div>
        </div>
        <div class="skill-card">
          <div class="skill-category">Design / HW</div>
          <div class="skill-items">
            <span class="skill-chip">3D CAD</span>
            <span class="skill-chip">FDM 3D프린팅</span>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div class="footer">INSTRUCTOR PROFILE · HUH YOUNGJIN · AI &amp; EMBEDDED SYSTEMS</div>
</div>
</body>
</html>
