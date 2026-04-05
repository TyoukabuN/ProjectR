const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

/**
 * Codex Review Skill
 * 调用本地 Codex CLI 审查内容
 */

// Codex CLI 路径
const CODEX_PATH = 'D:\\CodexCLI\\node_modules\\.bin\\codex.cmd';

function main(args) {
    const content = args.content || args._[0];
    const focus = args.focus || 'general';
    
    if (!content) {
        console.error('请提供要审查的内容或文件路径');
        process.exit(1);
    }

    // 构建审查提示词
    const focusPrompts = {
        architecture: '请重点审查架构设计的合理性',
        performance: '请重点审查性能方面是否存在问题',
        security: '请重点审查安全性隐患',
        completeness: '请重点审查是否有遗漏或不完善的地方',
        general: '请全面审查这个方案或代码'
    };

    let prompt = focusPrompts[focus] || focusPrompts.general;
    let targetContent = content;

    // 检查是否是文件路径
    if (fs.existsSync(content)) {
        const fullPath = path.resolve(content);
        console.log(`正在审查文件: ${fullPath}`);
        
        // 切换到文件所在目录执行
        const workDir = path.dirname(fullPath);
        const fileName = path.basename(fullPath);
        
        try {
            const result = execSync(
                `"${CODEX_PATH}" exec "${prompt}" --file "${fileName}"`,
                { 
                    cwd: workDir,
                    encoding: 'utf-8',
                    timeout: 120000
                }
            );
            console.log(result);
        } catch (error) {
            console.error('Codex 执行失败:', error.message);
            if (error.stdout) console.log(error.stdout);
            if (error.stderr) console.error(error.stderr);
        }
    } else {
        // 直接审查文本内容
        console.log('正在审查内容...');
        
        try {
            const result = execSync(
                `"${CODEX_PATH}" exec "${prompt}: ${content}"`,
                { 
                    cwd: process.cwd(),
                    encoding: 'utf-8',
                    timeout: 120000
                }
            );
            console.log(result);
        } catch (error) {
            console.error('Codex 执行失败:', error.message);
            if (error.stdout) console.log(error.stdout);
            if (error.stderr) console.error(error.stderr);
        }
    }
}

// 解析参数
const args = process.argv.slice(2).reduce((acc, arg, i, arr) => {
    if (arg.startsWith('--')) {
        const key = arg.slice(2);
        acc[key] = arr[i + 1] && !arr[i + 1].startsWith('--') ? arr[i + 1] : true;
    } else if (!acc._) {
        acc._ = [arg];
    } else {
        acc._.push(arg);
    }
    return acc;
}, {});

main(args);
