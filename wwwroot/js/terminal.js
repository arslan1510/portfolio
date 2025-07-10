// Terminal Emulator JavaScript
class Terminal {
    constructor() {
        this.history = [];
        this.historyIndex = -1;
        this.currentDirectory = '~';
        this.username = 'guest';
        this.hostname = 'portfolio';
        this.commands = this.initializeCommands();
        this.outputElement = null;
        this.inputElement = null;
        this.isProcessing = false;
        
        // Initialize when DOM is ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.initialize());
        } else {
            this.initialize();
        }
    }

    initialize() {
        this.outputElement = document.getElementById('terminal-output');
        this.inputElement = document.getElementById('terminal-input');
        this.timeElement = document.getElementById('terminal-time');
        this.welcomeElement = document.getElementById('terminal-welcome');
        
        if (!this.outputElement || !this.inputElement || !this.timeElement || !this.welcomeElement) {
            return;
        }

        // Set up event listeners
        this.inputElement.addEventListener('keydown', (e) => this.handleKeyDown(e));
        
        // Focus on input
        this.inputElement.focus();
        
        // Initialize and start time display
        this.updateTime();
        setInterval(() => this.updateTime(), 1000);
        
        // Display welcome message
        this.displayWelcome();
    }

    initializeCommands() {
        return {
            clear: {
                description: 'Clear the terminal screen',
                usage: 'clear',
                execute: () => this.cmdClear()
            }
        };
    }

    handleKeyDown(event) {
        switch(event.key) {
            case 'Enter':
                event.preventDefault();
                this.processCommand();
                break;
            case 'ArrowUp':
                event.preventDefault();
                this.navigateHistory(-1);
                break;
            case 'ArrowDown':
                event.preventDefault();
                this.navigateHistory(1);
                break;
            case 'Tab':
                event.preventDefault();
                this.autoComplete();
                break;
            case 'l':
                if (event.ctrlKey) {
                    event.preventDefault();
                    this.cmdClear();
                }
                break;
        }
    }

    async processCommand() {
        if (this.isProcessing) return;
        
        const input = this.inputElement.value.trim();
        if (!input) return;

        this.isProcessing = true;
        
        // Add to history
        this.history.push(input);
        this.historyIndex = this.history.length;
        
        // Display command
        this.addLine(`${this.getPrompt()}${input}`, 'command');
        
        // Clear input
        this.inputElement.value = '';
        
        // Parse and execute command
        const [cmd, ...args] = input.split(' ');
        await this.executeCommand(cmd.toLowerCase(), args);
        
        this.isProcessing = false;
        this.inputElement.focus();
    }

    async executeCommand(cmd, args) {
        // First check if it's a client-side command
        if (this.commands[cmd]) {
            try {
                await this.commands[cmd].execute(args);
            } catch (error) {
                this.addLine(`Error: ${error.message}`, 'error');
            }
        } else if (cmd) {
            // Try server-side command
            try {
                const response = await fetch('/api/terminal/command', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ command: cmd, args: args })
                });
                
                const result = await response.json();
                
                if (result.status === 'success') {
                    if (result.output === 'CLEAR_TERMINAL') {
                        this.cmdClear();
                    } else if (result.output && result.output.startsWith('OPEN_URL:')) {
                        const url = result.output.substring('OPEN_URL:'.length);
                        this.addLine(`Opening ${url}...`, 'info');
                        window.open(url, '_blank');
                    } else if (result.output && result.output.startsWith('OPEN_PROJECT:')) {
                        const projectNum = result.output.substring('OPEN_PROJECT:'.length);
                        this.openProject(parseInt(projectNum));
                    } else if (result.output) {
                        // Check if output contains HTML tags
                        if (result.output.includes('<div') || result.output.includes('<span')) {
                            this.addHTML(result.output);
                        } else if (cmd === 'contact' && result.output.includes('Email:')) {
                            // Handle contact command output with special formatting
                            this.displayContactOutput(result.output);
                        } else {
                            this.addLine(result.output);
                        }
                    }
                    
                    // Handle special data responses
                    if (result.data) {
                        if (cmd === 'github') {
                            this.displayProjectsFromServer(result.data);
                        } else if (cmd === 'skills') {
                            this.displaySkillsFromServer(result.data);
                        } else if (cmd === 'contact') {
                            this.displayContactFromServer(result.data);
                        }
                    }
                } else {
                    this.addLine(result.output || 'Command failed', 'error');
                }
            } catch (error) {
                this.addLine(`Command not found: ${cmd}. Type 'help' for available commands.`, 'error');
            }
        }
    }

    navigateHistory(direction) {
        const newIndex = this.historyIndex + direction;
        
        if (newIndex >= 0 && newIndex < this.history.length) {
            this.historyIndex = newIndex;
            this.inputElement.value = this.history[newIndex];
        } else if (newIndex >= this.history.length) {
            this.historyIndex = this.history.length;
            this.inputElement.value = '';
        }
    }

    autoComplete() {
        const input = this.inputElement.value.toLowerCase();
        const matches = Object.keys(this.commands).filter(cmd => cmd.startsWith(input));
        
        if (matches.length === 1) {
            this.inputElement.value = matches[0] + ' ';
        } else if (matches.length > 1) {
            this.addLine(`${this.getPrompt()}${this.inputElement.value}`, 'command');
            this.addLine(matches.join('  '), 'info');
        }
    }

    // Utility functions
    getPrompt() {
        return `${this.username}@${this.hostname} ${this.currentDirectory}$ `;
    }

    addLine(text, className = 'output') {
        const line = document.createElement('div');
        line.className = `terminal-line ${className}`;
        line.textContent = text;
        this.outputElement.appendChild(line);
        this.scrollToBottom();
    }

    addHTML(html, className = 'output') {
        const line = document.createElement('div');
        line.className = `terminal-line ${className}`;
        line.innerHTML = html;
        this.outputElement.appendChild(line);
        this.scrollToBottom();
    }

    scrollToBottom() {
        if (this.outputElement) {
            this.outputElement.scrollTop = this.outputElement.scrollHeight;
        }
    }

    displayWelcome() {
        const asciiArt = `
        
     █████╗ ██╗     ███████╗██████╗ ██╗  ██╗███╗   ██╗██╗   ██╗██╗     ██╗          ██████╗ ███████╗██╗   ██╗
    ██╔══██╗██║     ██╔════╝██╔══██╗██║  ██║████╗  ██║██║   ██║██║     ██║          ██╔══██╗██╔════╝██║   ██║
    ███████║██║     █████╗  ██████╔╝███████║██╔██╗ ██║██║   ██║██║     ██║          ██║  ██║█████╗  ██║   ██║
    ██╔══██║██║     ██╔══╝  ██╔═══╝ ██╔══██║██║╚██╗██║██║   ██║██║     ██║          ██║  ██║██╔══╝  ╚██╗ ██╔╝
    ██║  ██║███████╗███████╗██║     ██║  ██║██║ ╚████║╚██████╔╝███████╗███████╗ ██╗ ██████╔╝███████╗ ╚████╔╝ 
    ╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝╚══════╝ ╚═╝ ╚═════╝ ╚══════╝  ╚═══╝  
        
    `;
        
        this.welcomeElement.innerHTML = `
            <pre class="ascii-art">${asciiArt}</pre>
            <div style="text-align: left; margin-bottom: 1rem;">
                <div class="terminal-line info">Portfolio v0.0.1</div>
                <div class="terminal-line dim">Type 'help' for available commands</div>
            </div>
        `;
    }

    // Time update method
    updateTime() {
        const now = new Date();
        const timeString = now.toLocaleTimeString('en-US', { 
            hour12: false, 
            hour: '2-digit', 
            minute: '2-digit',
            second: '2-digit'
        });
        this.timeElement.textContent = timeString;
    }

    // Command implementations
    cmdClear() {
        this.outputElement.innerHTML = '';
    }

    displayProjectsFromServer(projects) {
        if (!projects || projects.length === 0) {
            this.addLine('No projects found.', 'warning');
            return;
        }
        
        // Create projects grid HTML
        let projectsHtml = '<div class="projects-grid">';
        
        projects.forEach((project, index) => {
            // Handle F# Project format (camelCase after JSON serialization)
            const title = project.title || 'Unknown Project';
            const description = project.description || 'No description available';
            const technologies = project.technologies || [];
            const githubUrl = project.gitHubUrl || '#';
            
            // Format technologies as a string
            const techString = technologies.length > 0 ? technologies.join(', ') : 'N/A';
            
            projectsHtml += `<div class="project-card"><div class="project-title">${title}</div><div class="project-description">${description}</div><div class="project-tech">${techString}</div><div class="project-links"><a href="${githubUrl}" target="_blank" class="project-link">View Code</a></div></div>`;
        });
        
        projectsHtml += '</div>';
        
        // Add the HTML to the terminal
        this.addHTML(projectsHtml);
        
        // Store projects for open command
        this.lastProjects = projects;
    }

    displaySkillsFromServer(skills) {
        // Skills data comes as a Map/Dictionary from F#
        
        // Create skills grid HTML
        let skillsHtml = '<div class="skills-grid">';
        
        Object.entries(skills).forEach(([category, skillList]) => {
            if (Array.isArray(skillList)) {
                // Add brackets, colors, and logos around each skill and join with spaces
                const skillsString = skillList.map(skill => {
                    const skillInfo = this.getSkillInfo(skill);
                    return `<span class="skill-tag ${skillInfo.type}">[${skillInfo.icon} ${skill}]</span>`;
                }).join(' ');
                
                skillsHtml += `<div class="skill-category"><div class="skill-name">${category.toUpperCase()}:</div><div class="skill-values">${skillsString}</div></div>`;
            }
        });
        
        skillsHtml += '</div>';
        
        // Add the HTML to the terminal
        this.addHTML(skillsHtml);
    }

    displayContactFromServer(contactData) {
        if (!contactData) {
            this.addLine('No contact information available.', 'warning');
            return;
        }
        
        // Create contact section HTML
        let contactHtml = '<div class="contact-section">';
        
        // Handle the contact data structure
        if (contactData.intro) {
            contactHtml += `<div class="contact-intro">${contactData.intro}</div>`;
        }
        
        if (contactData.email) {
            contactHtml += `<div class="contact-email">Email: <a href="mailto:${contactData.email}" class="terminal-link">${contactData.email}</a></div>`;
        }
        
        if (contactData.github) {
            contactHtml += `<div class="contact-github">GitHub: <a href="${contactData.github}" target="_blank" class="terminal-link">${contactData.github}</a></div>`;
        }
        
        if (contactData.linkedin) {
            contactHtml += `<div class="contact-linkedin">LinkedIn: <a href="${contactData.linkedin}" target="_blank" class="terminal-link">${contactData.linkedin}</a></div>`;
        }
        
        if (contactData.twitter) {
            contactHtml += `<div class="contact-twitter">Twitter: <a href="${contactData.twitter}" target="_blank" class="terminal-link">${contactData.twitter}</a></div>`;
        }
        
        contactHtml += '</div>';
        
        // Add the HTML to the terminal
        this.addHTML(contactHtml);
    }

    displayContactOutput(output) {
        // Parse the plain text contact output and convert to styled HTML
        const lines = output.split('\n');
        let contactHtml = '<div class="contact-section">';
        
        lines.forEach(line => {
            const trimmedLine = line.trim();
            if (!trimmedLine) return;
            
            if (trimmedLine.startsWith('Email:')) {
                const email = trimmedLine.replace('Email:', '').trim();
                contactHtml += `<div class="contact-email">Email: <a href="mailto:${email}" class="terminal-link">${email}</a></div>`;
            } else if (trimmedLine.startsWith('GitHub:')) {
                const github = trimmedLine.replace('GitHub:', '').trim();
                const githubUrl = github.startsWith('http') ? github : `https://${github}`;
                contactHtml += `<div class="contact-github">GitHub: <a href="${githubUrl}" target="_blank" class="terminal-link">${github}</a></div>`;
            } else if (trimmedLine.startsWith('LinkedIn:')) {
                const linkedin = trimmedLine.replace('LinkedIn:', '').trim();
                const linkedinUrl = linkedin.startsWith('http') ? linkedin : `https://${linkedin}`;
                contactHtml += `<div class="contact-linkedin">LinkedIn: <a href="${linkedinUrl}" target="_blank" class="terminal-link">${linkedin}</a></div>`;
            } else if (trimmedLine.startsWith('Twitter:')) {
                const twitter = trimmedLine.replace('Twitter:', '').trim();
                const twitterUrl = twitter.startsWith('http') ? twitter : `https://${twitter}`;
                contactHtml += `<div class="contact-twitter">Twitter: <a href="${twitterUrl}" target="_blank" class="terminal-link">${twitter}</a></div>`;
            } else if (trimmedLine.length > 0 && !trimmedLine.includes(':')) {
                // This is likely the closing message
                contactHtml += `<div class="contact-intro">${trimmedLine}</div>`;
            }
        });
        
        contactHtml += '</div>';
        
        // Add the HTML to the terminal
        this.addHTML(contactHtml);
    }

    getSkillInfo(skill) {
        // Mapping of skills to their types, colors, and icons using Noto Sans characters
        const skillMap = {
            // Programming Languages
            'Python': { type: 'language', icon: '' },
            'F#': { type: 'language', icon: '' },
            'Haskell': { type: 'language', icon: '' },
            
            // Frameworks & Libraries
            'Oxpecker': { type: 'framework', icon: '󱗆' },
            '.NET': { type: 'framework', icon: '󰪮' },
            'FastAPI': { type: 'framework', icon: '' },
            'NumPy': { type: 'framework', icon: '' },
            'Pandas': { type: 'framework', icon: '' },
            'PyTorch': { type: 'framework', icon: '' },
            
            // Databases
            'Neo4j': { type: 'database', icon: '' },
            'PostgreSQL': { type: 'database', icon: '' },
            
            // Cloud & DevOps
            'AWS': { type: 'cloud', icon: '' },
            'Docker': { type: 'devops', icon: '' },
            
            // Tools
            'Git': { type: 'tool', icon: '' },
            'Fish': { type: 'tool', icon: '' },
            'Linux': { type: 'os', icon: '󰣇' },
            
            // Mathematics
            'Category Theory': { type: 'math', icon: '󰊕' },
            'Analysis': { type: 'math', icon: '∫' },
            'Linear & Abstract Algebra': { type: 'math', icon: '∇' },
            'Statistics': { type: 'math', icon: '' },
            'Graph Theory': { type: 'math', icon: '󱁉' },
            'Combinatorics': { type: 'math', icon: '' }
        };

        return skillMap[skill] || { type: 'default', icon: '•' };
    }

    openProject(projectNum) {
        if (!this.lastProjects || this.lastProjects.length === 0) {
            this.addLine('No projects available. Run \'github\' command first.', 'error');
            return;
        }

        const index = projectNum - 1;
        if (index < 0 || index >= this.lastProjects.length) {
            this.addLine(`Error: Invalid project number. Choose between 1 and ${this.lastProjects.length}`, 'error');
            return;
        }

        const project = this.lastProjects[index];
        
        // Handle camelCase naming (after JSON serialization)
        const title = project.title || 'Unknown Project';
        const githubUrl = project.gitHubUrl;

        if (!githubUrl || githubUrl === '#' || githubUrl === '') {
            this.addLine(`Error: No URL found for project ${title}`, 'error');
            this.addLine(`Available properties: ${Object.keys(project).join(', ')}`, 'dim');
            return;
        }

        this.addLine(`Opening ${title} at ${githubUrl}...`, 'info');
        window.open(githubUrl, '_blank');
    }

    openProjectCard(githubUrl, title) {
        if (!githubUrl || githubUrl === '#' || githubUrl === '') {
            this.addLine(`Error: No URL found for project ${title}`, 'error');
            return;
        }

        this.addLine(`Opening ${title} at ${githubUrl}...`, 'info');
        window.open(githubUrl, '_blank');
    }
}

// Initialize terminal when script loads
const terminal = new Terminal(); 